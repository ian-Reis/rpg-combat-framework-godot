using Godot;
using Helpers;
using Interfaces;
using Constants;

namespace Resources.states;

[GlobalClass]
public partial class PlayerStateJump : PlayerState
{
    public override void Enter(ISystemLogicContext context)
    {
        AnimationTreeHelper.SetTreeCondition(context, AnimationParams.IsJump, true);
    }

    public override void PhysicsUpdate(ISystemLogicContext context, float delta)
    {
        if (context?.Pawn is not CharacterBody3D pawn) return;

        MovementHelper.ApplyMovement(context, delta, ignoreAirControl: false, canRun: false);
        pawn.MoveAndSlide();

        if (pawn.IsOnFloor())
            StateMachineHelper.ChangeState(context, PlayerStateNames.Idle);
    }

    public override void Exit(ISystemLogicContext context)
    {
        AnimationTreeHelper.SetTreeCondition(context, AnimationParams.IsJump, false);
    }
}
