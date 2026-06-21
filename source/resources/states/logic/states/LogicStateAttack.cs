using Godot;
using Handlers;
using Helpers;
using Constants;
using Components;
using Data;

namespace Resources.states;

[GlobalClass]
public partial class LogicStateAttack : LogicState
{
    [Export] public string     FirstAttackAnimState = "attack1";
    [Export] public bool       UseRootMotion        = true;
    [Export] public bool       LockMovement         = false;
    [Export] public AttackData AttackData;
    [Export] public string[]   InterruptibleBy      = [];

    public override void Enter(LogicStateMachineComponent sm)
    {
        sm.Pawn.AnimationSM?.ChangeState(FirstAttackAnimState);
        sm.Pawn.Hitbox?.Activate(AttackData);
    }

    public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    {
        PhysicsHandler.ApplyGravity(sm.Pawn, delta);

        if (UseRootMotion)
            ApplyRootMotion(sm);
        else if (!LockMovement)
            MovementHandler.ApplyMovement(sm.Pawn, delta);

        MovementHandler.MoveAndSlide(sm.Pawn);

        if (CanInterrupt(LogicStateNames.Airborne) && sm.Pawn != null && !sm.Pawn.IsOnFloor())
        {
            sm.ChangeState(LogicStateNames.Airborne);
            return;
        }

        var animSM = sm.Pawn.AnimationSM;
        if (animSM != null && animSM.CurrentStateName.StartsWith("attack")) return;

        sm.ChangeState(InputHelper.GetInputDirection().Length() > 0f
            ? LogicStateNames.Walk
            : LogicStateNames.Idle);
    }

    public override void HandleInput(LogicStateMachineComponent sm, InputEvent @event)
    {
        if (CanInterrupt(LogicStateNames.Jump)
            && @event.IsActionPressed("jump")
            && sm.Pawn != null
            && sm.Pawn.IsOnFloor())
        {
            sm.ChangeState(LogicStateNames.Jump);
        }
    }

    public override void Exit(LogicStateMachineComponent sm)
    {
        sm.Pawn.Hitbox?.Deactivate();
    }

    private bool CanInterrupt(string stateName)
        => InterruptibleBy != null && System.Array.IndexOf(InterruptibleBy, stateName) >= 0;

    private static void ApplyRootMotion(LogicStateMachineComponent sm)
    {
        if (sm.Pawn == null) return;
        var rootVel = sm.Pawn.AnimationSM?.CurrentSnapshot.RootMotionVelocity ?? Vector3.Zero;
        sm.Pawn.Velocity = new Vector3(rootVel.X, sm.Pawn.Velocity.Y, rootVel.Z);
    }
}
