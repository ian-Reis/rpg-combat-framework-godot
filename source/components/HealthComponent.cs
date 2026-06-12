using System.Diagnostics;
using Godot;

namespace Components;

[GlobalClass]
public partial class HealthComponent : Node
{
    [Signal] public delegate void HealthChangedEventHandler(float current, float max);
    [Signal] public delegate void DamageTakenEventHandler(float amount, Vector3 source);
    [Signal] public delegate void DiedEventHandler();
    [Signal] public delegate void HealedEventHandler(float amount);
    [Signal] public delegate void RevivedEventHandler();
    [Signal] public delegate void IFramesStartedEventHandler();
    [Signal] public delegate void IFramesEndedEventHandler();

    // ── Health ────────────────────────────────────────────────────────────────

    [ExportGroup("Health")]
    // When true, uses Stats.MaxHealth. When false, uses the MaxHealthOverride below.
    [Export] public bool  UseStatsMaxHealth  = true;
    [Export] public float MaxHealthOverride  = 100f;

    // ── Defense ───────────────────────────────────────────────────────────────

    [ExportGroup("Defense")]
    [Export] public float Defense        = 0f;   // flat reduction applied before percent
    [Export] public float DefensePercent = 0f;   // 0..1 — fraction of damage absorbed after flat
    [Export] public float MinDamage      = 1f;   // minimum damage that always gets through

    // ── Invincibility ─────────────────────────────────────────────────────────

    [ExportGroup("Invincibility")]
    [Export] public float IFramesDuration = 0f;    // seconds of invincibility after each hit
    [Export] public bool  StartInvincible = false; // begin the scene fully invincible

    // ── Regeneration ──────────────────────────────────────────────────────────

    [ExportGroup("Regeneration")]
    [Export] public float RegenPerSecond = 0f;  // health recovered per second (0 = disabled)
    [Export] public float RegenDelay     = 3f;  // seconds after last hit before regen starts

    // ── Public state ──────────────────────────────────────────────────────────

    public float Current          { get; private set; }
    public bool  IsDead           { get; private set; }
    public bool  IsInvincible     { get; private set; }
    public float NormalizedHealth => Max > 0f ? Mathf.Clamp(Current / Max, 0f, 1f) : 0f;

    public float Max => UseStatsMaxHealth
        ? (_owner?.Stats?.MaxHealth ?? MaxHealthOverride)
        : MaxHealthOverride;

    // ── Private state ─────────────────────────────────────────────────────────

    private float _iFramesTimer  = 0f;
    private float _regenTimer    = 0f;
    private SystemLogicComponents _owner;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    public override void _Ready()
    {
        _owner = GetParentOrNull<SystemLogicComponents>();
        Debug.Assert(_owner != null, "HealthComponent must be a child of SystemLogicComponents");

        Current      = Max;
        IsInvincible = StartInvincible;
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;

        TickIFrames(dt);
        TickRegen(dt);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public void TakeDamage(float amount, Vector3 source = default)
    {
        if (IsDead || IsInvincible || amount <= 0f) return;

        float effective = CalculateDamage(amount);
        Current = Mathf.Max(Current - effective, 0f);

        EmitSignal(SignalName.DamageTaken, effective, source);
        EmitSignal(SignalName.HealthChanged, Current, Max);

        _regenTimer = RegenDelay;

        if (IFramesDuration > 0f)
            StartIFrames();

        if (Current <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f || Current >= Max) return;

        Current = Mathf.Min(Current + amount, Max);
        EmitSignal(SignalName.Healed, amount);
        EmitSignal(SignalName.HealthChanged, Current, Max);
    }

    public void Revive(float? healthAmount = null)
    {
        if (!IsDead) return;

        IsDead  = false;
        Current = Mathf.Clamp(healthAmount ?? Max, 0f, Max);
        EmitSignal(SignalName.Revived);
        EmitSignal(SignalName.HealthChanged, Current, Max);
    }

    public void SetHealth(float value)
    {
        Current = Mathf.Clamp(value, 0f, Max);
        EmitSignal(SignalName.HealthChanged, Current, Max);
    }

    public void SetInvincible(bool value)
    {
        if (IsInvincible == value) return;
        IsInvincible = value;
        EmitSignal(value ? SignalName.IFramesStarted : SignalName.IFramesEnded);
    }

    // ── Private ───────────────────────────────────────────────────────────────

    private float CalculateDamage(float raw)
    {
        float after = Mathf.Max(raw - Defense, 0f);
        after -= after * Mathf.Clamp(DefensePercent, 0f, 1f);
        return Mathf.Max(after, raw > 0f ? MinDamage : 0f);
    }

    private void StartIFrames()
    {
        IsInvincible = true;
        _iFramesTimer = IFramesDuration;
        EmitSignal(SignalName.IFramesStarted);
    }

    private void TickIFrames(float dt)
    {
        if (!IsInvincible || StartInvincible || _iFramesTimer <= 0f) return;

        _iFramesTimer -= dt;
        if (_iFramesTimer <= 0f)
        {
            IsInvincible = false;
            EmitSignal(SignalName.IFramesEnded);
        }
    }

    private void TickRegen(float dt)
    {
        if (IsDead || RegenPerSecond <= 0f || Current >= Max) return;

        if (_regenTimer > 0f)
        {
            _regenTimer -= dt;
            return;
        }

        Heal(RegenPerSecond * dt);
    }

    private void Die()
    {
        IsDead = true;
        EmitSignal(SignalName.Died);
    }
}
