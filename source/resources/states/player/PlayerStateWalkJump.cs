using Godot;
using Helpers;
using Components;
using Constants;

namespace Resources.states;

[GlobalClass]
public partial class PlayerStateWalkJump : PlayerState
{
    public override void Enter(SystemLogicComponents owner)
    {
        AnimationTreeHelper.SetTreeCondition(owner, AnimationParams.IsWalkJump, true);
    }

    public override void PhysicsUpdate(SystemLogicComponents owner, float delta)
    {
        if (owner?.Pawn is not CharacterBody3D pawn) return;

        MovementHelper.ApplyMovement(owner, delta);
        pawn.MoveAndSlide();

        if (pawn.IsOnFloor())
            StateMachineHelper.ChangeState(owner, PlayerStateNames.Idle);
    }

    public override void Exit(SystemLogicComponents owner)
    {
        AnimationTreeHelper.SetTreeCondition(owner, AnimationParams.IsWalkJump, false);
    }
}
