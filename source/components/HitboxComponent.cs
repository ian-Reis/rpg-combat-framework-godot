using System.Collections.Generic;
using Godot;
using Data;

namespace Components;

// Area3D placed under the weapon bone or model.
// Detects HurtboxComponent (collision layer 2) on other entities.
[GlobalClass]
public partial class HitboxComponent : Area3D
{
    [Signal] public delegate void HitLandedEventHandler(HurtboxComponent target, AttackData data);

    [Export] public AttackData AttackData;

    public bool IsActive { get; private set; } = false;

    private Pawn                             _owner;
    private readonly HashSet<HurtboxComponent> _hitThisSwing = new();

    public override void _Ready()
    {
        CollisionLayer = 0;
        CollisionMask  = 2;
        Monitoring     = false;

        _owner = FindOwner();
        _owner?.RegisterComponent(this);
        AreaEntered += OnAreaEntered;
    }

    public void Activate(AttackData data = null)
    {
        if (data != null) AttackData = data;
        IsActive   = true;
        Monitoring = true;
        _hitThisSwing.Clear();
    }

    public void Deactivate()
    {
        IsActive   = false;
        Monitoring = false;
    }

    public void ResetSwing() => _hitThisSwing.Clear();

    // ── Private ───────────────────────────────────────────────────────────────

    private void OnAreaEntered(Area3D area)
    {
        if (!IsActive || AttackData == null) return;
        if (area is not HurtboxComponent hurtbox) return;
        if (!hurtbox.IsActive) return;
        if (hurtbox.LogicOwner == _owner) return;
        if (_hitThisSwing.Contains(hurtbox)) return;

        _hitThisSwing.Add(hurtbox);

        Vector3 dir = (hurtbox.GlobalPosition - GlobalPosition).Normalized();
        hurtbox.ReceiveHit(AttackData, dir);
        EmitSignal(SignalName.HitLanded, hurtbox, AttackData);
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
