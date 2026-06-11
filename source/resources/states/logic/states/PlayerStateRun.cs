using Godot;
using Handlers;
using Helpers;
using Interfaces;
using Constants;
using Components;

namespace Resources.states;

[GlobalClass]
public partial class PlayerStateRun : PlayerState
{
    public override void Enter(StateMachineComponent stateMachineComponent) { }

    public override void PhysicsUpdate(StateMachineComponent stateMachineComponent  , float delta)
    {

       if (stateMachineComponent?.systemLogicContext is not ISystemLogicContext logicContext) return;

        JumpHandler.ApplyJump(logicContext);
        PhysicsHandler.ApplyGravity(logicContext, delta);
        MovementHandler.ApplyMovement(logicContext, delta);
        MovementHandler.MoveAndSlide(logicContext);

        var inputDir = InputHelper.GetInputDirection();
        bool isMoving  = inputDir.Length() > 0f;
        bool isRunning = Input.IsActionPressed("run");

        if (!isMoving)  { StateMachineHelper.ChangeState(stateMachineComponent, PlayerStateNames.Idle); return; }
        if (!isRunning) { StateMachineHelper.ChangeState(stateMachineComponent, PlayerStateNames.Walk); return; }
    }

    public override void Exit(StateMachineComponent stateMachineComponent) { }
}
