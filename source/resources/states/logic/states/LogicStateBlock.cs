using Godot;
using Handlers;
using Helpers;
using Constants;
using Components;

namespace Resources.states;

[GlobalClass]
public partial class LogicStateBlock : LogicState
{
    [Export] public string BlockAnimState  = "block";
    [Export] public float  DamageReduction = 0.7f;
    [Export] public float  ParryWindow     = 0.2f;

    private const string ParryTimerMeta = "block_parry_timer";

    public override void Enter(LogicStateMachineComponent sm)
    {
        var hurtbox = sm.Pawn.Hurtbox;
        if (hurtbox != null)
        {
            hurtbox.DamageMultiplier    = 1f - DamageReduction;
            hurtbox.IsParryWindowActive = true;
            sm.Pawn?.SetMeta(ParryTimerMeta, ParryWindow);
        }

        sm.Pawn.AnimationSM?.ChangeState(BlockAnimState);
    }

    public override void Update(LogicStateMachineComponent sm, float delta)
    {
        if (sm.Pawn == null) return;

        float timer = sm.Pawn.GetMeta(ParryTimerMeta, 0f).AsSingle();
        if (timer <= 0f) return;

        timer -= delta;
        sm.Pawn.SetMeta(ParryTimerMeta, timer);

        if (timer <= 0f && sm.Pawn.Hurtbox != null)
            sm.Pawn.Hurtbox.IsParryWindowActive = false;
    }

    public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    {
        PhysicsHandler.ApplyGravity(sm.Pawn, delta);
        MovementHandler.ApplyMovement(sm.Pawn, delta);
        MovementHandler.MoveAndSlide(sm.Pawn);

        if (!Input.IsActionPressed("block"))
            sm.ChangeState(InputHelper.GetInputDirection().Length() > 0f
                ? LogicStateNames.Walk
                : LogicStateNames.Idle);
    }

    public override void Exit(LogicStateMachineComponent sm)
    {
        var hurtbox = sm.Pawn.Hurtbox;
        if (hurtbox == null) return;
        hurtbox.DamageMultiplier    = 1f;
        hurtbox.IsParryWindowActive = false;
    }
}
