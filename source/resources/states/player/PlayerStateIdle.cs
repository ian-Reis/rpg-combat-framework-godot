using Godot;
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
        
        var pawn = context.Pawn as CharacterBody3D;
        // JumpHelper.ApplyJump(stateMachineComponent);
        // JumpHelper.JumpTravel(stateMachineComponent);
        pawn.MoveAndSlide();

        if (MovementHelper.GetInputDirection().Length() > 0f)
            stateMachineComponent.ChangeState(PlayerStateNames.Walk);
    }

    public override void Exit(StateMachineComponent stateMachineComponent)
    {
        AnimationTreeHelper.SetTreeCondition(stateMachineComponent, AnimationParams.IsIdle, false);
    }
}
