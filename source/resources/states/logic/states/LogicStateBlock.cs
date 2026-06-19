using Godot;
using Handlers;
using Helpers;
using Interfaces;
using Constants;
using Components;

namespace Resources.states;

// Block state: reduces incoming damage and opens a parry window at entry.
// Requires "block" action in the project's InputMap.
// Connect to HurtboxComponent.ParrySuccessful to handle perfect-parry feedback.
[GlobalClass]
public partial class LogicStateBlock : LogicState
{
    [Export] public string BlockAnimState  = "block";
    [Export] public float  DamageReduction = 0.7f;  // fraction of damage absorbed (0..1)
    [Export] public float  ParryWindow     = 0.2f;  // seconds after entry where hit = parry

    private const string ParryTimerMeta = "block_parry_timer";

    // public override void Enter(LogicStateMachineComponent sm)
    // {
    //     if (sm?.systemLogicContext is not ISystemLogicContext context) return;

    //     var hurtbox = context.GetComponent<HurtboxComponent>();
    //     if (hurtbox != null)
    //     {
    //         hurtbox.DamageMultiplier    = 1f - DamageReduction;
    //         hurtbox.IsParryWindowActive = true;
    //         context.Pawn?.SetMeta(ParryTimerMeta, ParryWindow);
    //     }

    //     context.AnimationStateMachineComponent?.ChangeState(BlockAnimState);
    // }

    // public override void Update(LogicStateMachineComponent sm, float delta)
    // {
    //     if (sm?.systemLogicContext is not ISystemLogicContext context) return;
    //     if (context.Pawn == null) return;

    //     float timer = context.Pawn.GetMeta(ParryTimerMeta, 0f).AsSingle();
    //     if (timer <= 0f) return;

    //     timer -= delta;
    //     context.Pawn.SetMeta(ParryTimerMeta, timer);

    //     if (timer <= 0f)
    //     {
    //         var hurtbox = context.GetComponent<HurtboxComponent>();
    //         if (hurtbox != null) hurtbox.IsParryWindowActive = false;
    //     }
    // }

    // public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    // {
    //     if (sm?.systemLogicContext is not ISystemLogicContext context) return;

    //     PhysicsHandler.ApplyGravity(context, delta);
    //     MovementHandler.ApplyMovement(context, delta);
    //     MovementHandler.MoveAndSlide(context);

    //     if (!Input.IsActionPressed("block"))
    //         sm.ChangeState(InputHelper.GetInputDirection().Length() > 0f
    //             ? LogicStateNames.Walk
    //             : LogicStateNames.Idle);
    // }

    // public override void Exit(LogicStateMachineComponent sm)
    // {
    //     if (sm?.systemLogicContext is not ISystemLogicContext context) return;

    //     var hurtbox = context.GetComponent<HurtboxComponent>();
    //     if (hurtbox == null) return;

    //     hurtbox.DamageMultiplier    = 1f;
    //     hurtbox.IsParryWindowActive = false;
    // }
}
