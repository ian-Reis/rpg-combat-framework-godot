using Godot;
using Handlers;
using Helpers;
using Interfaces;
using Constants;
using Components;

namespace Resources.states;

// Knockback is stored in pawn metadata by HurtboxComponent before ChangeState("hit"),
// so this resource is safe to share across multiple entities.
[GlobalClass]
public partial class LogicStateHit : LogicState
{
    [Export] public string HitAnimState   = "hit";
    [Export] public float  HitDuration    = 0.4f;
    [Export] public float  KnockbackDecay = 20f;
    // Override recovery state. Leave empty to use player default (Walk/Idle by input).
    [Export] public string RecoverState   = "";

    private const string HitTimerMeta    = "hit_timer";
    private const string HitKnockbackMeta = "hit_knockback";

    public override void Enter(LogicStateMachineComponent sm)
    {
        if (sm?.systemLogicContext is not ISystemLogicContext context) return;

        context.Pawn?.SetMeta(HitTimerMeta, HitDuration);
        context.AnimationStateMachineComponent?.ChangeState(HitAnimState);
    }

    public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    {
        if (sm?.systemLogicContext is not ISystemLogicContext context) return;
        if (context.Pawn == null) return;

        var knockback = context.Pawn.GetMeta(HitKnockbackMeta, Vector3.Zero).AsVector3();

        if (context.Pawn is CharacterBody3D cb)
            cb.Velocity = new Vector3(knockback.X, cb.Velocity.Y, knockback.Z);

        context.Pawn.SetMeta(HitKnockbackMeta,
            knockback.MoveToward(Vector3.Zero, KnockbackDecay * delta));

        PhysicsHandler.ApplyGravity(context, delta);
        MovementHandler.MoveAndSlide(context);

        float timer = context.Pawn.GetMeta(HitTimerMeta, 0f).AsSingle() - delta;
        context.Pawn.SetMeta(HitTimerMeta, timer);

        if (timer <= 0f)
        {
            if (!string.IsNullOrEmpty(RecoverState))
                sm.ChangeState(RecoverState);
            else
                sm.ChangeState(InputHelper.GetInputDirection().Length() > 0f
                    ? LogicStateNames.Walk
                    : LogicStateNames.Idle);
        }
    }

    public override void Exit(LogicStateMachineComponent sm)
    {
        if (sm?.systemLogicContext is not ISystemLogicContext context) return;
        context.Pawn?.SetMeta(HitKnockbackMeta, Vector3.Zero);
    }
}
