using System.Collections.Generic;
using System.Linq;
using Godot;
using Data;

namespace Components;

[GlobalClass]
public partial class StatusEffectComponent : Node
{
    [Signal] public delegate void EffectAppliedEventHandler(StatusEffect effect);
    [Signal] public delegate void EffectRemovedEventHandler(StatusEffect effect);

    private Pawn _pawn;
    private readonly Dictionary<string, (StatusEffect effect, float timeRemaining)> _active = new();

    public HealthComponent Health => _pawn?.Health;

    public override void _Ready()
    {
        _pawn = GetParent<Pawn>();
    }

    public override void _Process(double delta)
    {
        if (_active.Count == 0) return;

        float dt      = (float)delta;
        var   toRemove = new List<string>();

        foreach (var key in new List<string>(_active.Keys))
        {
            if (!_active.TryGetValue(key, out var entry)) continue;

            entry.effect.OnTick(this, dt);

            float remaining = entry.timeRemaining - dt;
            if (remaining <= 0f)
                toRemove.Add(key);
            else
                _active[key] = (entry.effect, remaining);
        }

        foreach (var key in toRemove)
            Remove(key);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public void Apply(StatusEffect effect)
    {
        if (effect == null || string.IsNullOrEmpty(effect.EffectId)) return;

        if (_active.ContainsKey(effect.EffectId))
        {
            _active[effect.EffectId] = (effect, effect.Duration);
            return;
        }

        _active[effect.EffectId] = (effect, effect.Duration);
        effect.OnApply(this);
        EmitSignal(SignalName.EffectApplied, effect);
    }

    public void Remove(string effectId)
    {
        if (!_active.TryGetValue(effectId, out var entry)) return;
        entry.effect.OnRemove(this);
        _active.Remove(effectId);
        EmitSignal(SignalName.EffectRemoved, entry.effect);
    }

    public bool Has(string effectId)                  => _active.ContainsKey(effectId);
    public bool HasType(StatusEffect.EffectType type) => _active.Values.Any(e => e.effect.Type == type);
    public bool IsStunned                             => HasType(StatusEffect.EffectType.Stun);

    public float GetSpeedMultiplier()
    {
        float mult = 1f;
        foreach (var kvp in _active)
        {
            if (kvp.Value.effect is SlowEffect slow)
                mult *= slow.SpeedMultiplier;
        }
        return Mathf.Clamp(mult, 0.1f, 1f);
    }
}
