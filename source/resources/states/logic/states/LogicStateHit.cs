using Godot;
using Handlers;
using Helpers;
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
    [Export] public string RecoverState   = "";

    private const string HitTimerMeta     = "hit_timer";
    private const string HitKnockbackMeta = "hit_knockback";

    public override void Enter(LogicStateMachineComponent sm)
    {
        sm.Pawn?.SetMeta(HitTimerMeta, HitDuration);
        sm.Pawn.AnimationSM?.ChangeState(HitAnimState);
    }

    public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    {
        if (sm.Pawn == null) return;

        var knockback = sm.Pawn.GetMeta(HitKnockbackMeta, Vector3.Zero).AsVector3();
        sm.Pawn.Velocity = new Vector3(knockback.X, sm.Pawn.Velocity.Y, knockback.Z);

        sm.Pawn.SetMeta(HitKnockbackMeta,
            knockback.MoveToward(Vector3.Zero, KnockbackDecay * delta));

        PhysicsHandler.ApplyGravity(sm.Pawn, delta);
        MovementHandler.MoveAndSlide(sm.Pawn);

        float timer = sm.Pawn.GetMeta(HitTimerMeta, 0f).AsSingle() - delta;
        sm.Pawn.SetMeta(HitTimerMeta, timer);

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
        sm.Pawn?.SetMeta(HitKnockbackMeta, Vector3.Zero);
    }
}
