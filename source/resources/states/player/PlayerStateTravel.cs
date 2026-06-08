using Godot;
using Helpers;
using Interfaces;
using Constants;

namespace Resources.states;

[GlobalClass]
public partial class PlayerStateTravel : PlayerState
{
    public override void Enter(IPlayerStateContext context)
    {
        AnimationTreeHelper.SetTreeCondition(context, AnimationParams.IsTravel, true);
    }

    public override void PhysicsUpdate(IPlayerStateContext context, float delta)
    {
        if (context?.Pawn is not CharacterBody3D pawn) return;

        JumpHelper.JumpTravel(context);
        pawn.MoveAndSlide();

        if (PlanetsHelper.ChangedPlanet(context))
            StateMachineHelper.ChangeState(context, PlayerStateNames.Idle);
    }

    public override void Exit(IPlayerStateContext context)
    {
        AnimationTreeHelper.SetTreeCondition(context, AnimationParams.IsTravel, false);
    }
}
