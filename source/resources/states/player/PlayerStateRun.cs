using Godot;
using Helpers;
using Interfaces;
using Constants;
using Components;

namespace Resources.states;

[GlobalClass]
public partial class PlayerStateRun : PlayerState
{
    public override void Enter(StateMachineComponent stateMachineComponent)
    {
        AnimationTreeHelper.SetTreeCondition(stateMachineComponent, AnimationParams.IsRun, true);
    }

    public override void PhysicsUpdate(StateMachineComponent stateMachineComponent  , float delta)
    {

       if (stateMachineComponent?.systemLogicContext is not ISystemLogicContext logicContext) return;

        var pawn = logicContext.Pawn as CharacterBody3D;
        if (pawn == null) return;

        JumpHelper.ApplyJump(logicContext);
        MovementHelper.ApplyMovement(logicContext, delta);
        pawn.MoveAndSlide();

        var inputDir = MovementHelper.GetInputDirection();
        bool isMoving  = inputDir.Length() > 0f;
        bool isRunning = Input.IsActionPressed("run");

        if (!isMoving)  { StateMachineHelper.ChangeState(stateMachineComponent, PlayerStateNames.Idle); return; }
        if (!isRunning) { StateMachineHelper.ChangeState(stateMachineComponent, PlayerStateNames.Walk); return; }
    }

    public override void Exit(StateMachineComponent stateMachineComponent)
    {
        AnimationTreeHelper.SetTreeCondition(stateMachineComponent, AnimationParams.IsRun, false);
    }
}
