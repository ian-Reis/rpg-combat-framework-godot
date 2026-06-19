using Godot;
using Handlers;
using Interfaces;
using Constants;
using Components;

namespace Resources.states;

// Moves toward AIMemoryComponent.LastKnownPosition when the target is lost.
// Exits to IdleState when the position is reached or memory fades.
[GlobalClass]
public partial class LogicStateAIInvestigate : LogicState
{
    [Export] public float  ArrivalThreshold = 0.8f;
    [Export] public string DoneState        = LogicStateNames.AiIdle;

    // public override void Enter(LogicStateMachineComponent sm)
    // {
    //     sm?.systemLogicContext?.AnimationStateMachineComponent?.ChangeState("locomotion");
    // }

    // public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    // {
    //     if (sm?.systemLogicContext is not ISystemLogicContext context) return;
    //     if (context.Pawn is not CharacterBody3D cb) return;

    //     var memory = context.GetComponent<AIMemoryComponent>();
    //     if (memory == null || !memory.HasMemory)
    //     {
    //         sm.ChangeState(DoneState);
    //         return;
    //     }

    //     // If target was re-acquired, the BT will switch state — nothing to do here.

    //     Vector3 dir = memory.LastKnownPosition - cb.GlobalPosition;
    //     dir.Y = 0f;

    //     if (dir.Length() <= ArrivalThreshold)
    //     {
    //         sm.ChangeState(DoneState);
    //         return;
    //     }

    //     dir = dir.Normalized();
    //     float speed = context.Stats?.WalkSpeed ?? 3f;
    //     cb.Velocity = new Vector3(dir.X * speed, cb.Velocity.Y, dir.Z * speed);
    //     cb.Rotation = new Vector3(cb.Rotation.X, Mathf.Atan2(-dir.X, -dir.Z), cb.Rotation.Z);

    //     PhysicsHandler.ApplyGravity(context, delta);
    //     MovementHandler.MoveAndSlide(context);
    // }
}
