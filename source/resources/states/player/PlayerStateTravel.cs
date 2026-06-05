using Godot;
using Helpers;
using Components;
using Constants;

namespace Resources.states;

[GlobalClass]
public partial class PlayerStateTravel : PlayerState
{
    public override void Enter(SystemLogicComponents owner)
    {
        AnimationTreeHelper.SetTreeCondition(owner, AnimationParams.IsTravel, true);
    }

    public override void PhysicsUpdate(SystemLogicComponents owner, float delta)
    {
        if (owner?.Pawn is not CharacterBody3D pawn) return;

        JumpHelper.JumpTravel(owner);
        pawn.MoveAndSlide();

        if (PlanetsHelper.ChangedPlanet(owner))
            StateMachineHelper.ChangeState(owner, PlayerStateNames.Idle);
    }

    public override void Exit(SystemLogicComponents owner)
    {
        AnimationTreeHelper.SetTreeCondition(owner, AnimationParams.IsTravel, false);
    }
}
