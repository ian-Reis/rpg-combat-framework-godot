using Godot;
using Handlers;
using Components;

namespace Resources.states;

[GlobalClass]
public partial class LogicStateAIIdle : LogicState
{
    public override void Enter(LogicStateMachineComponent sm)
    {
        sm.Pawn.AnimationSM?.ChangeState("locomotion");
    }

    public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    {
        var pawn = sm.Pawn;
        if (pawn == null) return;

        float decel = pawn.Stats?.Deceleration ?? 8f;
        pawn.Velocity = new Vector3(
            Mathf.MoveToward(pawn.Velocity.X, 0f, decel * delta),
            pawn.Velocity.Y,
            Mathf.MoveToward(pawn.Velocity.Z, 0f, decel * delta)
        );

        PhysicsHandler.ApplyGravity(pawn, delta);
        MovementHandler.MoveAndSlide(pawn);
    }
}
