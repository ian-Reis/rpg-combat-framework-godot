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
    public override void Enter(StateMachineComponent stateMachineComponent)
    {
        AnimationTreeHelper.SetTreeCondition(stateMachineComponent, AnimationParams.IsIdle, true);
    }

    public override void PhysicsUpdate(StateMachineComponent stateMachineComponent, float delta)
    {
        if (stateMachineComponent?.systemLogicContext is not ISystemLogicContext context) return;

        // JumpHelper.ApplyJump(context);
        // JumpHelper.JumpTravel(context);

        switch (context.Pawn)
        {
            case CharacterBody3D:
                MovementHandler.MoveAndSlide(context);
                break;
            case RigidBody3D rigidBody:
                RigidBodyHelper.BrakeHorizontal(rigidBody, 0f, 15f * delta);
                break;
        }

        if (MovementHandler.GetInputDirection().Length() > 0f)
            stateMachineComponent.ChangeState(PlayerStateNames.Walk);
    }

    public override void Exit(StateMachineComponent stateMachineComponent)
    {
        AnimationTreeHelper.SetTreeCondition(stateMachineComponent, AnimationParams.IsIdle, false);
    }
}
