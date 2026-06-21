using Godot;
using Data;

namespace Components;

[GlobalClass]
public partial class DodgeComponent : Node
{
    [Signal] public delegate void DodgeStartedEventHandler();
    [Signal] public delegate void DodgeEndedEventHandler();

    [ExportGroup("Stats")]
    [Export] public DodgeStats Stats;

    public bool IsDodging    { get; private set; } = false;
    public bool IsOnCooldown => _cooldownTimer > 0f;

    private Pawn    _pawn;
    private float   _cooldownTimer  = 0f;
    private float   _dodgeTimer     = 0f;
    private Vector3 _dodgeDirection = Vector3.Zero;

    public override void _Ready()
    {
        _pawn = GetParent<Pawn>();
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;

        if (_cooldownTimer > 0f)
            _cooldownTimer -= dt;

        if (!IsDodging) return;

        _dodgeTimer -= dt;
        if (_dodgeTimer <= 0f)
            EndDodge();
    }

    public bool TryDodge(Vector3 direction)
    {
        if (IsDodging || IsOnCooldown || Stats == null) return false;

        _dodgeDirection = direction.IsZeroApprox()
            ? (_pawn != null ? -_pawn.Transform.Basis.Z : Vector3.Forward)
            : direction.Normalized();

        IsDodging      = true;
        _dodgeTimer    = Stats.Duration;
        _cooldownTimer = Stats.Cooldown;

        _pawn?.Health?.SetInvincible(true);
        EmitSignal(SignalName.DodgeStarted);
        return true;
    }

    public Vector3 GetDodgeVelocity() => Stats != null ? _dodgeDirection * Stats.Speed : Vector3.Zero;

    private void EndDodge()
    {
        IsDodging = false;
        _pawn?.Health?.SetInvincible(false);
        EmitSignal(SignalName.DodgeEnded);
    }
}
