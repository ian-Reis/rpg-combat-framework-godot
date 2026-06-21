using Godot;
using Handlers;
using Constants;
using Components;

namespace Resources.states;

[GlobalClass]
public partial class LogicStateAIPatrol : LogicState
{
    [Export] public float  WaypointThreshold = 0.6f;
    [Export] public string NoWaypointsState  = LogicStateNames.AiIdle;

    public override void Enter(LogicStateMachineComponent sm)
    {
        sm.Pawn.AnimationSM?.ChangeState("locomotion");
    }

    public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    {
        var pawn = sm.Pawn;
        if (pawn == null) return;

        var patrol = pawn.Patrol;
        if (patrol == null || !patrol.HasWaypoints)
        {
            sm.ChangeState(NoWaypointsState);
            return;
        }

        if (patrol.IsAtWaypoint(pawn, WaypointThreshold))
            patrol.NextWaypoint();

        Node3D wp = patrol.CurrentWaypoint;
        if (wp == null) { sm.ChangeState(NoWaypointsState); return; }

        Vector3 dir = wp.GlobalPosition - pawn.GlobalPosition;
        dir.Y = 0f;

        if (dir.LengthSquared() > 0.01f)
        {
            dir = dir.Normalized();
            float speed = pawn.Stats?.WalkSpeed ?? 3f;
            pawn.Velocity = new Vector3(dir.X * speed, pawn.Velocity.Y, dir.Z * speed);
            pawn.Rotation = new Vector3(pawn.Rotation.X, Mathf.Atan2(-dir.X, -dir.Z), pawn.Rotation.Z);
        }

        PhysicsHandler.ApplyGravity(pawn, delta);
        MovementHandler.MoveAndSlide(pawn);
    }
}
