using Godot;
using Helpers;
using Interfaces;
using Constants;

namespace Resources.states;

[GlobalClass]
public partial class PlayerStateIdle : PlayerState
{
    public override void Enter(ISystemLogicContext context)
    {
        AnimationTreeHelper.SetTreeCondition(context, AnimationParams.IsIdle, true);
    }

    public override void PhysicsUpdate(ISystemLogicContext context, float delta)
    {
        if (context?.Pawn is not CharacterBody3D pawn) return;

        JumpHelper.ApplyJump(context);
        JumpHelper.JumpTravel(context);
        pawn.MoveAndSlide();

        if (MovementHelper.GetInputDirection().Length() > 0f)
            StateMachineHelper.ChangeState(context, PlayerStateNames.Walk);
    }

    public override void Exit(ISystemLogicContext context)
    {
        AnimationTreeHelper.SetTreeCondition(context, AnimationParams.IsIdle, false);
    }
}
