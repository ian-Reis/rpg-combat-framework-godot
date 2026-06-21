using Godot;
using Handlers;
using Helpers;
using Constants;
using Components;

namespace Resources.states;

[GlobalClass]
public partial class LogicStateDodge : LogicState
{
    [Export] public string DodgeAnimState = "dodge";
    [Export] public bool   UseRootMotion  = true;

    public override void Enter(LogicStateMachineComponent sm)
    {
        sm.Pawn.AnimationSM?.ChangeState(DodgeAnimState);
    }

    public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    {
        PhysicsHandler.ApplyGravity(sm.Pawn, delta);

        if (UseRootMotion)
        {
            ApplyRootMotion(sm);
            MovementHandler.MoveAndSlide(sm.Pawn);

            var animSM = sm.Pawn.AnimationSM;
            if (animSM != null && animSM.CurrentStateName == DodgeAnimState) return;
        }
        else
        {
            MovementHandler.MoveAndSlide(sm.Pawn);
        }

        ExitToIdle(sm);
    }

    private static void ApplyRootMotion(LogicStateMachineComponent sm)
    {
        if (sm.Pawn == null) return;
        var rootVel = sm.Pawn.AnimationSM?.CurrentSnapshot.RootMotionVelocity ?? Vector3.Zero;
        sm.Pawn.Velocity = new Vector3(rootVel.X, sm.Pawn.Velocity.Y, rootVel.Z);
    }

    private static void ExitToIdle(LogicStateMachineComponent sm)
    {
        sm.ChangeState(InputHelper.GetInputDirection().Length() > 0f
            ? LogicStateNames.Walk
            : LogicStateNames.Idle);
    }
}
