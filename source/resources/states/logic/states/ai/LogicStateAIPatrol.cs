using Godot;
using Handlers;
using Interfaces;
using Constants;
using Components;

namespace Resources.states;

// Moves between waypoints defined in AIPatrolComponent.
// Automatically advances to the next waypoint when close enough.
[GlobalClass]
public partial class LogicStateAIPatrol : LogicState
{
    [Export] public float  WaypointThreshold = 0.6f;
    [Export] public string NoWaypointsState  = LogicStateNames.AiIdle;

    // public override void Enter(LogicStateMachineComponent sm)
    // {
    //     sm?.systemLogicContext?.AnimationStateMachineComponent?.ChangeState("locomotion");
    // }

    // public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    // {
    //     if (sm?.systemLogicContext is not ISystemLogicContext context) return;
    //     if (context.Pawn is not CharacterBody3D cb) return;

    //     var patrol = context.GetComponent<AIPatrolComponent>();
    //     if (patrol == null || !patrol.HasWaypoints)
    //     {
    //         sm.ChangeState(NoWaypointsState);
    //         return;
    //     }

    //     if (patrol.IsAtWaypoint(cb, WaypointThreshold))
    //         patrol.NextWaypoint();

    //     Node3D wp = patrol.CurrentWaypoint;
    //     if (wp == null) { sm.ChangeState(NoWaypointsState); return; }

    //     Vector3 dir = wp.GlobalPosition - cb.GlobalPosition;
    //     dir.Y = 0f;

    //     if (dir.LengthSquared() > 0.01f)
    //     {
    //         dir = dir.Normalized();
    //         float speed = context.Stats?.WalkSpeed ?? 3f;
    //         cb.Velocity = new Vector3(dir.X * speed, cb.Velocity.Y, dir.Z * speed);
    //         cb.Rotation = new Vector3(cb.Rotation.X, Mathf.Atan2(-dir.X, -dir.Z), cb.Rotation.Z);
    //     }

    //     PhysicsHandler.ApplyGravity(context, delta);
    //     MovementHandler.MoveAndSlide(context);
    // }
}
