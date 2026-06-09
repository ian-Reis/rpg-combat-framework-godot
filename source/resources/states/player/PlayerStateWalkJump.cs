using Godot;
using Helpers;
using Interfaces;
using Constants;

namespace Resources.states;

[GlobalClass]
public partial class PlayerStateWalkJump : PlayerState
{
    // public override void Enter(ISystemLogicContext context)
    // {
    //     AnimationTreeHelper.SetTreeCondition(context, AnimationParams.IsWalkJump, true);
    // }

    // public override void PhysicsUpdate(ISystemLogicContext context, float delta)
    // {
    //     if (context?.Pawn is not CharacterBody3D pawn) return;

    //     MovementHelper.ApplyMovement(context, delta);
    //     pawn.MoveAndSlide();

    //     if (pawn.IsOnFloor())
    //     {
    //         var inputDir = MovementHelper.GetInputDirection();
    //         bool isMoving = inputDir.Length() > 0f;
    //         bool isRunning = Input.IsActionPressed("run");

    //         if (!isMoving)
    //             StateMachineHelper.ChangeState(context, PlayerStateNames.Idle);
    //         else if (isRunning)
    //             StateMachineHelper.ChangeState(context, PlayerStateNames.Run);
    //         else
    //             StateMachineHelper.ChangeState(context, PlayerStateNames.Walk);
    //     }
    // }

    // public override void Exit(ISystemLogicContext context)
    // {
    //     AnimationTreeHelper.SetTreeCondition(context, AnimationParams.IsWalkJump, false);
    // }
}
