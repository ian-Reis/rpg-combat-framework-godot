using Godot;
using Handlers;
using Interfaces;
using Constants;
using Components;

namespace Resources.states;

// Moves away from AIDetectionComponent.CurrentTarget at RunSpeed.
// Exits to SafeState when no target is detected.
[GlobalClass]
public partial class LogicStateAIFlee : LogicState
{
    [Export] public string SafeState = LogicStateNames.AiIdle;

    public override void Enter(LogicStateMachineComponent sm)
    {
        sm?.systemLogicContext?.AnimationStateMachineComponent?.ChangeState("locomotion");
    }

    public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    {
        if (sm?.systemLogicContext is not ISystemLogicContext context) return;
        if (context.Pawn is not CharacterBody3D cb) return;

        var target = context.GetComponent<AIDetectionComponent>()?.CurrentTarget;
        if (target == null || !IsInstanceValid(target))
        {
            sm.ChangeState(SafeState);
            return;
        }

        Vector3 away = cb.GlobalPosition - target.GlobalPosition;
        away.Y = 0f;

        if (away.LengthSquared() > 0.01f)
        {
            away = away.Normalized();
            float speed = context.Stats?.RunSpeed ?? 6f;
            cb.Velocity = new Vector3(away.X * speed, cb.Velocity.Y, away.Z * speed);
            cb.Rotation = new Vector3(cb.Rotation.X, Mathf.Atan2(-away.X, -away.Z), cb.Rotation.Z);
        }

        PhysicsHandler.ApplyGravity(context, delta);
        MovementHandler.MoveAndSlide(context);
    }
}
