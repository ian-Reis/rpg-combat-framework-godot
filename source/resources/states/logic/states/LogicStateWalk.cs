using Godot;
using Handlers;
using Helpers;
using Interfaces;
using Constants;
using Components;

namespace Resources.states;

[GlobalClass]
public partial class LogicStateWalk : LogicState
{
    public override void Enter(LogicStateMachineComponent stateMachineComponent) { }

    public override void PhysicsUpdate(LogicStateMachineComponent stateMachineComponent, float delta)
    {
        if (stateMachineComponent?.systemLogicContext is not ISystemLogicContext logicContext) return;

        // JumpHandler.ApplyJump(logicContext);
        PhysicsHandler.ApplyGravity(logicContext, delta);
        MovementHandler.ApplyMovement(logicContext, delta);
        MovementHandler.MoveAndSlide(logicContext);

        var inputDir = InputHelper.GetInputDirection();
        bool isMoving  = inputDir.Length() > 0f;
        bool isRunning = Input.IsActionPressed("run");

        if (!isMoving)  { LogicStateMachineHelper.ChangeState(stateMachineComponent, LogicStateNames.Idle); return; }
        if (isRunning)  { LogicStateMachineHelper.ChangeState(stateMachineComponent, LogicStateNames.Run);  return; }
    }

    public override void Exit(LogicStateMachineComponent stateMachineComponent) { }
}
