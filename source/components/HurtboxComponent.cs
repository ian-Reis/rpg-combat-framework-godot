using System.Diagnostics;
using Godot;
using Data;
using Constants;

namespace Components;

// Area3D placed anywhere under the entity's CharacterBody3D.
// Self-registers to the nearest SystemLogicComponents ancestor so that
// context.GetComponent<HurtboxComponent>() works from logic states.
// Collision layer 2 (hitboxes use mask 2 to detect it).
[GlobalClass]
public partial class HurtboxComponent : Area3D
{
    [Signal] public delegate void HitReceivedEventHandler(AttackData data, Vector3 direction);
    [Signal] public delegate void ParrySuccessfulEventHandler(AttackData data, Vector3 direction);

    [Export] public bool IsActive = true;

    public float DamageMultiplier    { get; set; } = 1f;
    public bool  IsParryWindowActive { get; set; } = false;

    public SystemLogicComponents LogicOwner => _owner;

    private SystemLogicComponents _owner;
    private HealthComponent       _health;

    public override void _Ready()
    {
        CollisionLayer = 2;
        CollisionMask  = 0;

        _owner = FindOwner();
        _owner?.RegisterComponent(this);

        Debug.Assert(_owner != null,
            $"[HurtboxComponent] '{Name}' could not find a SystemLogicComponents ancestor.");
    }

    public void ReceiveHit(AttackData data, Vector3 direction)
    {
        if (!IsActive || data == null) return;

        if (IsParryWindowActive && data.CanParry)
        {
            EmitSignal(SignalName.ParrySuccessful, data, direction);
            return;
        }

        _health ??= _owner?.GetComponent<HealthComponent>();
        _health?.TakeDamage(data.Damage * DamageMultiplier, direction);

        var statusComp = _owner?.GetComponent<StatusEffectComponent>();
        if (statusComp != null)
            ApplyStatusEffects(statusComp, data);

        if (data.KnockbackForce > 0f)
            TriggerKnockback(direction.Normalized() * data.KnockbackForce);

        EmitSignal(SignalName.HitReceived, data, direction);
    }

    // ── Private ───────────────────────────────────────────────────────────────

    private void ApplyStatusEffects(StatusEffectComponent comp, AttackData data)
    {
        for (int i = 0; i < data.Effects.Length; i++)
        {
            if (data.Effects[i] == null) continue;
            float chance = i < data.EffectChances.Length ? data.EffectChances[i] : 1f;
            if (GD.Randf() <= chance)
                comp.Apply(data.Effects[i]);
        }
    }

    private void TriggerKnockback(Vector3 velocity)
    {
        if (_owner?.Pawn == null) return;
        _owner.Pawn.SetMeta("hit_knockback", velocity);
        _owner.LogicStateMachineComponent?.ChangeState(LogicStateNames.Hit);
    }

    private SystemLogicComponents FindOwner()
    {
        Node node = GetParent();
        while (node != null)
        {
            if (node is SystemLogicComponents slc) return slc;
            node = node.GetParent();
        }
        return null;
    }
}
