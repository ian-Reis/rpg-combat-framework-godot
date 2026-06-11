using Godot;
using Handlers;
using Helpers;
using Interfaces;
using Constants;
using Components;

namespace Resources.states;

[GlobalClass]
public partial class LogicStateIdle : LogicState
{
    public override void Enter(LogicStateMachineComponent stateMachineComponent) { }

    public override void PhysicsUpdate(LogicStateMachineComponent stateMachineComponent, float delta)
    {
        if (stateMachineComponent?.systemLogicContext is not ISystemLogicContext context) return;

        PhysicsHandler.ApplyGravity(context, delta);
        MovementHandler.ApplyMovement(context, delta);
        MovementHandler.MoveAndSlide(context);

        if (InputHelper.GetInputDirection().Length() > 0f)
            stateMachineComponent.ChangeState(LogicStateNames.Walk);
    }

    public override void Exit(LogicStateMachineComponent stateMachineComponent) { }
}
