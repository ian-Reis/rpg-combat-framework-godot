using Godot;
using Handlers;
using Helpers;
using Constants;
using Components;
using Data;

namespace Resources.states;

[GlobalClass]
public partial class LogicStateAIAttack : LogicState
{
    [Export] public string AttackAnimState  = "attack1";
    [Export] public bool   UseRootMotion    = false;
    [Export] public string AfterAttackState = LogicStateNames.AiChase;

    public override void Enter(LogicStateMachineComponent sm)
    {
        var attackData = PickAttack(sm);
        sm.Pawn.Hitbox?.Activate(attackData);
        sm.Pawn.AnimationSM?.ChangeState(AttackAnimState);
    }

    public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    {
        var pawn = sm.Pawn;
        if (pawn == null) return;

        PhysicsHandler.ApplyGravity(pawn, delta);

        if (UseRootMotion)
        {
            var vel = pawn.AnimationSM?.CurrentSnapshot.RootMotionVelocity ?? Vector3.Zero;
            pawn.Velocity = new Vector3(vel.X, pawn.Velocity.Y, vel.Z);
        }
        else
        {
            float decel = pawn.Stats?.Deceleration ?? 8f;
            pawn.Velocity = new Vector3(
                Mathf.MoveToward(pawn.Velocity.X, 0f, decel * delta),
                pawn.Velocity.Y,
                Mathf.MoveToward(pawn.Velocity.Z, 0f, decel * delta)
            );
        }

        MovementHandler.MoveAndSlide(pawn);

        var animSM = pawn.AnimationSM;
        if (animSM != null && AnimationTreeHelper.GetCurrentNode(animSM) == AttackAnimState) return;

        sm.ChangeState(AfterAttackState);
    }

    public override void Exit(LogicStateMachineComponent sm)
    {
        sm.Pawn.Hitbox?.Deactivate();
    }

    private static AttackData PickAttack(LogicStateMachineComponent sm)
    {
        var patterns = sm.Pawn.Brain?.EnemyData?.AttackPatterns;
        if (patterns == null || patterns.Length == 0) return null;
        return patterns[(int)(GD.Randi() % (uint)patterns.Length)];
    }
}
