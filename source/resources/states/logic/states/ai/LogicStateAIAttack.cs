using Godot;
using Handlers;
using Interfaces;
using Constants;
using Components;
using Data;
using Helpers;

namespace Resources.states;

// AI attack state: stops movement, triggers an attack from EnemyData.AttackPatterns,
// activates the hitbox, and returns to AfterAttackState when the animation finishes.
[GlobalClass]
public partial class LogicStateAIAttack : LogicState
{
    [Export] public string AttackAnimState   = "attack1";
    [Export] public bool   UseRootMotion     = false;
    [Export] public string AfterAttackState  = LogicStateNames.AiChase;

    public override void Enter(LogicStateMachineComponent sm)
    {
        if (sm?.systemLogicContext is not ISystemLogicContext context) return;

        var attackData = PickAttack(context);
        context.GetComponent<HitboxComponent>()?.Activate(attackData);
        context.AnimationStateMachineComponent?.ChangeState(AttackAnimState);
    }

    public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    {
        if (sm?.systemLogicContext is not ISystemLogicContext context) return;
        if (context.Pawn is not CharacterBody3D cb) return;

        PhysicsHandler.ApplyGravity(context, delta);

        if (UseRootMotion)
        {
            var vel = context.AnimationStateMachineComponent?.CurrentSnapshot.RootMotionVelocity ?? Vector3.Zero;
            cb.Velocity = new Vector3(vel.X, cb.Velocity.Y, vel.Z);
        }
        else
        {
            float decel = context.Stats?.Deceleration ?? 8f;
            cb.Velocity = new Vector3(
                Mathf.MoveToward(cb.Velocity.X, 0f, decel * delta),
                cb.Velocity.Y,
                Mathf.MoveToward(cb.Velocity.Z, 0f, decel * delta)
            );
        }

        MovementHandler.MoveAndSlide(context);

        var animSM = context.AnimationStateMachineComponent;
        if (animSM != null && AnimationTreeHelper.GetCurrentNode(animSM) == AttackAnimState) return;

        sm.ChangeState(AfterAttackState);
    }

    public override void Exit(LogicStateMachineComponent sm)
    {
        if (sm?.systemLogicContext is not ISystemLogicContext context) return;
        context.GetComponent<HitboxComponent>()?.Deactivate();
    }

    private static AttackData PickAttack(ISystemLogicContext context)
    {
        var patterns = context.GetComponent<BrainComponent>()?.EnemyData?.AttackPatterns;
        if (patterns == null || patterns.Length == 0) return null;
        return patterns[(int)(GD.Randi() % (uint)patterns.Length)];
    }
}
