using Godot;
using Handlers;
using Interfaces;
using Constants;
using Components;

namespace Resources.states;

// AI state that chases DetectionComponent.CurrentTarget.
// Handles movement, rotation, gravity, and MoveAndSlide.
// The BT only decides when to enter/exit this state via bt_set_logic_state.
[GlobalClass]
public partial class LogicStateAIChase : LogicState
{
    [Export] public string LostTargetState = LogicStateNames.Idle;

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
            sm.ChangeState(LostTargetState);
            return;
        }

        Vector3 dir = target.GlobalPosition - cb.GlobalPosition;
        dir.Y = 0f;

        if (dir.LengthSquared() > 0.01f)
        {
            dir = dir.Normalized();

            float speed = context.Stats?.WalkSpeed ?? 3f;
            cb.Velocity = new Vector3(dir.X * speed, cb.Velocity.Y, dir.Z * speed);

            // Rotate pawn to face movement direction.
            float targetAngle = Mathf.Atan2(-dir.X, -dir.Z);
            cb.Rotation = new Vector3(cb.Rotation.X, targetAngle, cb.Rotation.Z);
        }

        PhysicsHandler.ApplyGravity(context, delta);
        MovementHandler.MoveAndSlide(context);
    }
}
