using Godot;
using Data;
using Constants;

namespace Components;

// Area3D placed anywhere under the entity. FindOwner traverses up to find
// the ancestor Pawn so Area3D nesting under bones is unrestricted.
// Collision layer 2 — HitboxComponent uses mask 2 to detect it.
[GlobalClass]
public partial class HurtboxComponent : Area3D
{
    [Signal] public delegate void HitReceivedEventHandler(AttackData data, Vector3 direction);
    [Signal] public delegate void ParrySuccessfulEventHandler(AttackData data, Vector3 direction);

    [Export] public bool IsActive = true;

    public float DamageMultiplier    { get; set; } = 1f;
    public bool  IsParryWindowActive { get; set; } = false;

    public Pawn LogicOwner => _owner;

    private Pawn _owner;

    public override void _Ready()
    {
        CollisionLayer = 2;
        CollisionMask  = 0;
        _owner = FindOwner();
        _owner?.RegisterComponent(this);
    }

    public void ReceiveHit(AttackData data, Vector3 direction)
    {
        if (!IsActive || data == null) return;

        if (IsParryWindowActive && data.CanParry)
        {
            EmitSignal(SignalName.ParrySuccessful, data, direction);
            return;
        }

        _owner?.Health?.TakeDamage(data.Damage * DamageMultiplier, direction);

        var statusComp = _owner?.StatusFX;
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
        if (_owner == null) return;
        _owner.SetMeta("hit_knockback", velocity);
        _owner.LogicSM?.ChangeState(LogicStateNames.Hit);
    }

    private Pawn FindOwner()
    {
        Node node = GetParent();
        while (node != null)
        {
            if (node is Pawn pawn) return pawn;

            foreach (var child in node.GetChildren())
                if (child is Pawn p) return p;

            node = node.GetParent();
        }
        return null;
    }
}
