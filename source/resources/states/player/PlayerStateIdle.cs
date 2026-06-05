using Godot;
using Helpers;
using Components;
using Constants;

namespace Resources.states;

[GlobalClass]
public partial class PlayerStateIdle : PlayerState
{
    public override void Enter(SystemLogicComponents owner)
    {
        AnimationTreeHelper.SetTreeCondition(owner, AnimationParams.IsIdle, true);
    }

    public override void PhysicsUpdate(SystemLogicComponents owner, float delta)
    {
        if (owner?.Pawn is not CharacterBody3D pawn) return;

        JumpHelper.ApplyJump(owner);
        JumpHelper.JumpTravel(owner);
        pawn.MoveAndSlide();

        if (MovementHelper.GetInputDirection().Length() > 0f)
            StateMachineHelper.ChangeState(owner, PlayerStateNames.Walk);
    }

    public override void Exit(SystemLogicComponents owner)
    {
        AnimationTreeHelper.SetTreeCondition(owner, AnimationParams.IsIdle, false);
    }
}
