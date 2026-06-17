using Godot;
using Handlers;
using Helpers;
using Interfaces;
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
        if (sm?.systemLogicContext is not ISystemLogicContext context) return;

        var dodge = context.GetComponent<DodgeComponent>();
        if (dodge == null) { sm.ChangeState(LogicStateNames.Idle); return; }

        Vector3 dir = GetInputDirection(context);
        if (!dodge.TryDodge(dir)) { sm.ChangeState(LogicStateNames.Idle); return; }

        context.AnimationStateMachineComponent?.ChangeState(DodgeAnimState);
    }

    public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    {
        if (sm?.systemLogicContext is not ISystemLogicContext context) return;

        PhysicsHandler.ApplyGravity(context, delta);

        if (UseRootMotion)
        {
            ApplyRootMotion(context);
            MovementHandler.MoveAndSlide(context);

            // Exit when the animation SM leaves the dodge state.
            var animSM = context.AnimationStateMachineComponent;
            if (animSM != null && animSM.CurrentStateName == DodgeAnimState) return;
        }
        else
        {
            var dodge = context.GetComponent<DodgeComponent>();
            if (dodge == null || !dodge.IsDodging)
            {
                ExitToIdle(sm);
                return;
            }

            if (context.Pawn is CharacterBody3D cb)
            {
                var dv = dodge.GetDodgeVelocity();
                cb.Velocity = new Vector3(dv.X, cb.Velocity.Y, dv.Z);
            }

            MovementHandler.MoveAndSlide(context);
            return;
        }

        ExitToIdle(sm);
    }

    public override void Exit(LogicStateMachineComponent sm) { }

    private static void ApplyRootMotion(ISystemLogicContext context)
    {
        if (context.Pawn is not CharacterBody3D cb) return;
        var rootVel = context.AnimationStateMachineComponent?.CurrentSnapshot.RootMotionVelocity ?? Vector3.Zero;
        cb.Velocity = new Vector3(rootVel.X, cb.Velocity.Y, rootVel.Z);
    }

    private static void ExitToIdle(LogicStateMachineComponent sm)
    {
        sm.ChangeState(InputHelper.GetInputDirection().Length() > 0f
            ? LogicStateNames.Walk
            : LogicStateNames.Idle);
    }

    private static Vector3 GetInputDirection(ISystemLogicContext context)
    {
        Vector2 input = InputHelper.GetInputDirection();
        if (input.Length() < 0.1f) return Vector3.Zero;

        SpringArm3D spring = context.GetComponent<CameraComponent>()?.SpringArm;
        Basis       basis  = spring?.GlobalTransform.Basis ?? Basis.Identity;
        Vector3     up     = (context.Pawn as CharacterBody3D)?.UpDirection ?? Vector3.Up;

        Vector3 forward = (-basis.Z - up * (-basis.Z).Dot(up)).Normalized();
        Vector3 right   = ( basis.X - up * ( basis.X).Dot(up)).Normalized();

        return (right * input.X + forward * -input.Y).Normalized();
    }
}
