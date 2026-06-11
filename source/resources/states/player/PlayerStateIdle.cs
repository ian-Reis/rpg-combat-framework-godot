using Godot;
using Handlers;
using Helpers;
using Interfaces;
using Constants;
using Components;

namespace Resources.states;

[GlobalClass]
public partial class PlayerStateIdle : PlayerState
{
    public override void Enter(StateMachineComponent stateMachineComponent) { }

    public override void PhysicsUpdate(StateMachineComponent stateMachineComponent, float delta)
    {
        if (stateMachineComponent?.systemLogicContext is not ISystemLogicContext context) return;

        PhysicsHandler.ApplyGravity(context, delta);
        MovementHandler.ApplyMovement(context, delta);
        MovementHandler.MoveAndSlide(context);

        if (InputHelper.GetInputDirection().Length() > 0f)
            stateMachineComponent.ChangeState(PlayerStateNames.Walk);
    }

    public override void Exit(StateMachineComponent stateMachineComponent) { }
}
