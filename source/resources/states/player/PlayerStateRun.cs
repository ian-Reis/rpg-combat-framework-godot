using Godot;
using Helpers;
using Components;
using Constants;

namespace Resources.states;

[GlobalClass]
public partial class PlayerStateRun : PlayerState
{
    public override void Enter(SystemLogicComponents owner)
    {
        AnimationTreeHelper.SetTreeCondition(owner, AnimationParams.IsRun, true);
    }

    public override void PhysicsUpdate(SystemLogicComponents owner, float delta)
    {
        if (owner?.Pawn is not CharacterBody3D pawn) return;

        JumpHelper.ApplyJump(owner);
        MovementHelper.ApplyMovement(owner, delta);
        pawn.MoveAndSlide();

        var inputDir = MovementHelper.GetInputDirection();
        bool isMoving  = inputDir.Length() > 0f;
        bool isRunning = Input.IsActionPressed("run");

        if (!isMoving)  { StateMachineHelper.ChangeState(owner, PlayerStateNames.Idle); return; }
        if (!isRunning) { StateMachineHelper.ChangeState(owner, PlayerStateNames.Walk); return; }
    }

    public override void Exit(SystemLogicComponents owner)
    {
        AnimationTreeHelper.SetTreeCondition(owner, AnimationParams.IsRun, false);
    }
}
