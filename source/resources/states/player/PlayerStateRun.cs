using Godot;
using Helpers;
using Interfaces;
using Constants;

namespace Resources.states;

[GlobalClass]
public partial class PlayerStateRun : PlayerState
{
    public override void Enter(IPlayerStateContext context)
    {
        AnimationTreeHelper.SetTreeCondition(context, AnimationParams.IsRun, true);
    }

    public override void PhysicsUpdate(IPlayerStateContext context, float delta)
    {
        if (context?.Pawn is not CharacterBody3D pawn) return;

        JumpHelper.ApplyJump(context);
        MovementHelper.ApplyMovement(context, delta);
        pawn.MoveAndSlide();

        var inputDir = MovementHelper.GetInputDirection();
        bool isMoving  = inputDir.Length() > 0f;
        bool isRunning = Input.IsActionPressed("run");

        if (!isMoving)  { StateMachineHelper.ChangeState(context, PlayerStateNames.Idle); return; }
        if (!isRunning) { StateMachineHelper.ChangeState(context, PlayerStateNames.Walk); return; }
    }

    public override void Exit(IPlayerStateContext context)
    {
        AnimationTreeHelper.SetTreeCondition(context, AnimationParams.IsRun, false);
    }
}
