using Godot;
using Helpers;
using Components;
using Constants;

namespace Resources.states;

[GlobalClass]
public partial class PlayerStateJump : PlayerState
{
    public override void Enter(SystemLogicComponents owner)
    {
        owner.CanJump = true;
        AnimationTreeHelper.SetTreeCondition(owner, AnimationParams.IsJump, true);
    }

    public override void PhysicsUpdate(SystemLogicComponents owner, float delta)
    {
        if (owner?.Pawn is not CharacterBody3D pawn) return;

        MovementHelper.ApplyMovement(owner, delta, ignoreAirControl: false, canRun: false);
        pawn.MoveAndSlide();

        if (pawn.IsOnFloor())
            StateMachineHelper.ChangeState(owner, PlayerStateNames.Idle);
    }

    public override void Exit(SystemLogicComponents owner)
    {
        AnimationTreeHelper.SetTreeCondition(owner, AnimationParams.IsJump, false);
    }
}
