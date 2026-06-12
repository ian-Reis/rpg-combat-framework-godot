using Godot;
using Handlers;
using Helpers;
using Interfaces;
using Constants;
using Components;

namespace Resources.states;

[GlobalClass]
public partial class LogicStateAttack : LogicState
{
    [Export] public string   FirstAttackAnimState = "attack1";
    [Export] public bool     UseRootMotion        = true;
    [Export] public bool     LockMovement         = false;

    // Logic states that are allowed to interrupt this attack mid-animation.
    [Export] public string[] InterruptibleBy      = [];

    public override void Enter(LogicStateMachineComponent sm)
    {
        if (sm?.systemLogicContext is not ISystemLogicContext context) return;
        context.AnimationStateMachineComponent?.ChangeState(FirstAttackAnimState);
    }

    public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    {
        if (sm?.systemLogicContext is not ISystemLogicContext context) return;

        PhysicsHandler.ApplyGravity(context, delta);

        if (UseRootMotion)
            ApplyRootMotion(context);
        else if (!LockMovement)
            MovementHandler.ApplyMovement(context, delta);

        MovementHandler.MoveAndSlide(context);

        // Check physics-based interrupts (e.g. knocked into the air).
        if (CanInterrupt(LogicStateNames.Airborne) && context.Pawn is CharacterBody3D cb && !cb.IsOnFloor())
        {
            sm.ChangeState(LogicStateNames.Airborne);
            return;
        }

        // Wait until the animation SM finishes the entire combo chain.
        var animSM = context.AnimationStateMachineComponent;
        if (animSM != null && animSM.CurrentStateName.StartsWith("attack")) return;

        sm.ChangeState(InputHelper.GetInputDirection().Length() > 0f
            ? LogicStateNames.Walk
            : LogicStateNames.Idle);
    }

    public override void HandleInput(LogicStateMachineComponent sm, InputEvent @event)
    {
        if (sm?.systemLogicContext is not ISystemLogicContext context) return;

        if (CanInterrupt(LogicStateNames.Jump)
            && @event.IsActionPressed("jump")
            && context.Pawn is CharacterBody3D charBody
            && charBody.IsOnFloor())
        {
            sm.ChangeState(LogicStateNames.Jump);
            return;
        }
    }

    public override void Exit(LogicStateMachineComponent sm) { }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private bool CanInterrupt(string stateName)
        => InterruptibleBy != null && System.Array.IndexOf(InterruptibleBy, stateName) >= 0;

    private static void ApplyRootMotion(ISystemLogicContext context)
    {
        if (context.Pawn is not CharacterBody3D charBody) return;
        var rootVel = context.AnimationStateMachineComponent?.CurrentSnapshot.RootMotionVelocity ?? Vector3.Zero;
        charBody.Velocity = new Vector3(rootVel.X, charBody.Velocity.Y, rootVel.Z);
    }
}
