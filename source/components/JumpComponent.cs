using Godot;
using Classes.statics;

namespace Components;

[GlobalClass]
public partial class JumpComponent : Node
{
    [Signal] public delegate void JumpedEventHandler();
    [Signal] public delegate void LandedEventHandler();

    private Pawn _pawn;
    private float _jumpTimer  = 0f;
    private bool  _isJumping  = false;
    private bool  _wasOnFloor = false;

    public override void _Ready()
    {
        _pawn = GetParent<Pawn>();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_pawn == null || _pawn.Stats == null) return;

        float dt        = (float)delta;
        bool  isOnFloor = _pawn.IsOnFloor();

        if (isOnFloor && !_wasOnFloor)
            EmitSignal(SignalName.Landed);

        _wasOnFloor = isOnFloor;

        ProcessJump(_pawn, isOnFloor, dt);
        ProcessJumpTravel(_pawn, dt);
    }

    private void ProcessJump(Pawn pawn, bool isOnFloor, float dt)
    {
        Vector3 up        = GetUpDirection(pawn);
        float   vertSpeed = pawn.Velocity.Dot(up);

        if (Input.IsActionJustPressed("jump") && isOnFloor)
        {
            pawn.Velocity -= up * vertSpeed;
            pawn.Velocity += up * pawn.Stats.JumpForce;
            _isJumping = true;
            _jumpTimer = pawn.Stats.JumpHoldTime;
            EmitSignal(SignalName.Jumped);
        }

        if (Input.IsActionPressed("jump") && _isJumping && _jumpTimer > 0f)
        {
            pawn.Velocity += up * pawn.Stats.Gravity * dt;
            _jumpTimer -= dt;
        }

        if (Input.IsActionJustReleased("jump") && pawn.Velocity.Dot(up) > 0f)
        {
            float vert = pawn.Velocity.Dot(up);
            pawn.Velocity -= up * vert;
            pawn.Velocity += up * vert * pawn.Stats.CutJumpFactor;
            _isJumping = false;
        }
    }

    private void ProcessJumpTravel(Pawn pawn, float dt)
    {
        if (!Input.IsActionPressed("jet")) return;

        Vector3 up = GetUpDirection(pawn);
        pawn.Velocity -= up * pawn.Velocity.Dot(up);
        pawn.Velocity += up * pawn.Stats.JumpForce;
    }

    private static Vector3 GetUpDirection(Pawn pawn)
    {
        var planet = pawn.Get(EntityProps.CurrentPlanet).As<Node3D>();
        if (planet != null)
            return (pawn.GlobalPosition - planet.GlobalPosition).Normalized();

        return pawn.UpDirection.Normalized();
    }
}
