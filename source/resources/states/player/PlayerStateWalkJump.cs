using Godot;
using Helpers;
using Interfaces;
using Constants;

namespace Resources.states;

[GlobalClass]
public partial class PlayerStateWalkJump : PlayerState
{
    public override void Enter(IPlayerStateContext context)
    {
        AnimationTreeHelper.SetTreeCondition(context, AnimationParams.IsWalkJump, true);
    }

    public override void PhysicsUpdate(IPlayerStateContext context, float delta)
    {
        if (context?.Pawn is not CharacterBody3D pawn) return;

        MovementHelper.ApplyMovement(context, delta);
        pawn.MoveAndSlide();

        if (pawn.IsOnFloor())
            StateMachineHelper.ChangeState(context, PlayerStateNames.Idle);
    }

    public override void Exit(IPlayerStateContext context)
    {
        AnimationTreeHelper.SetTreeCondition(context, AnimationParams.IsWalkJump, false);
    }
}
