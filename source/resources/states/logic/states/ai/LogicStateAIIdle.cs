using Godot;
using Handlers;
using Interfaces;
using Components;

namespace Resources.states;

// AI idle: decelerates to a stop, applies gravity, and slides.
// No input checks — BT decides all transitions.
[GlobalClass]
public partial class LogicStateAIIdle : LogicState
{
    public override void Enter(LogicStateMachineComponent sm)
    {
        
    }

    public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    {
        // if (sm?.Pawn is not CharacterBody3D cb) return;

        // float decel = Pawn.Stats?.Deceleration ?? 8f;
        // cb.Velocity = new Vector3(
        //     Mathf.MoveToward(cb.Velocity.X, 0f, decel * delta),
        //     cb.Velocity.Y,
        //     Mathf.MoveToward(cb.Velocity.Z, 0f, decel * delta)
        // );

        // PhysicsHandler.ApplyGravity(context, delta);
        // MovementHandler.MoveAndSlide(context);
    }
}
