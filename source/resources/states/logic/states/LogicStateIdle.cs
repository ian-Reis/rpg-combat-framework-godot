using Godot;
using Handlers;
using Helpers;
using Constants;
using Components;

namespace Resources.states;

[GlobalClass]
public partial class LogicStateIdle : LogicState
{
    public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    {
        PhysicsHandler.ApplyGravity(sm.Pawn, delta);
        MovementHandler.ApplyMovement(sm.Pawn, delta);
        MovementHandler.MoveAndSlide(sm.Pawn);

        if (InputHelper.GetInputDirection().Length() > 0f)
            sm.ChangeState(LogicStateNames.Walk);
    }

    public override void HandleInput(LogicStateMachineComponent sm, InputEvent @event)
    {
        if (@event.IsActionPressed("attack"))
            sm.ChangeState(LogicStateNames.Attack);

        if (InputMap.HasAction("dodge") && @event.IsActionPressed("dodge"))
            sm.ChangeState(LogicStateNames.Dodge);

        if (InputMap.HasAction("block") && @event.IsActionPressed("block"))
            sm.ChangeState(LogicStateNames.Block);
    }
}
