using Godot;
using Handlers;
using Helpers;
using Interfaces;
using Constants;
using Components;

namespace Resources.states;

[GlobalClass]
public partial class PlayerStateWalk : PlayerState
{
    public override void Enter(StateMachineComponent stateMachineComponent)
    {
        AnimationTreeHelper.SetTreeCondition(stateMachineComponent, AnimationParams.IsWalk, true);
    }

    public override void PhysicsUpdate(StateMachineComponent stateMachineComponent, float delta)
    {
        if (stateMachineComponent?.systemLogicContext is not ISystemLogicContext logicContext) return;

        // JumpHelper.ApplyJump(logicContext);
        MovementHandler.ApplyMovement(logicContext, delta);
        MovementHandler.MoveAndSlide(logicContext);

        var inputDir = MovementHandler.GetInputDirection();
        bool isMoving  = inputDir.Length() > 0f;
        bool isRunning = Input.IsActionPressed("run");

        if (!isMoving)  { StateMachineHelper.ChangeState(stateMachineComponent, PlayerStateNames.Idle); return; }
        if (isRunning)  { StateMachineHelper.ChangeState(stateMachineComponent, PlayerStateNames.Run);  return; }
    }

    public override void Exit(StateMachineComponent stateMachineComponent)
    {
        AnimationTreeHelper.SetTreeCondition(stateMachineComponent, AnimationParams.IsWalk, false);
    }
}
