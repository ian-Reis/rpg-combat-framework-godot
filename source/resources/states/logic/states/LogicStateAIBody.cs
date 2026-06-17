using Godot;
using Handlers;
using Interfaces;
using Components;

namespace Resources.states;

// Passive physics state for AI-controlled entities.
// BT tasks own all behavioral decisions and set CharacterBody3D.Velocity each tick.
// This state only applies deceleration, gravity, and move_and_slide.
//
// Scene order requirement: BrainComponent (BTPlayer) must appear BEFORE
// LogicStateMachineComponent in the scene tree so BTPlayer sets velocity
// before this state consumes it.
[GlobalClass]
public partial class LogicStateAIBody : LogicState
{
    public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    {
        if (sm?.systemLogicContext is not ISystemLogicContext context) return;
        if (context.Pawn is not CharacterBody3D cb) return;

        float decel = context.Stats?.Deceleration ?? 8f;
        cb.Velocity = new Vector3(
            Mathf.MoveToward(cb.Velocity.X, 0f, decel * delta),
            cb.Velocity.Y,
            Mathf.MoveToward(cb.Velocity.Z, 0f, decel * delta)
        );

        PhysicsHandler.ApplyGravity(context, delta);
        MovementHandler.MoveAndSlide(context);
    }
}
