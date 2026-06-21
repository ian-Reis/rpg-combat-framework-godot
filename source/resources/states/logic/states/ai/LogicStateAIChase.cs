using Godot;
using Handlers;
using Constants;
using Components;

namespace Resources.states;

[GlobalClass]
public partial class LogicStateAIChase : LogicState
{
    [Export] public string LostTargetState = LogicStateNames.AiIdle;

    public override void Enter(LogicStateMachineComponent sm)
    {
        sm.Pawn.AnimationSM?.ChangeState("locomotion");
    }

    public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    {
        var pawn = sm.Pawn;
        if (pawn == null) return;

        var target = pawn.Detection?.CurrentTarget;
        if (target == null || !IsInstanceValid(target))
        {
            sm.ChangeState(LostTargetState);
            return;
        }

        Vector3 dir = target.GlobalPosition - pawn.GlobalPosition;
        dir.Y = 0f;

        if (dir.LengthSquared() > 0.01f)
        {
            dir = dir.Normalized();
            float speed = pawn.Stats?.WalkSpeed ?? 3f;
            pawn.Velocity = new Vector3(dir.X * speed, pawn.Velocity.Y, dir.Z * speed);
            pawn.Rotation = new Vector3(pawn.Rotation.X, Mathf.Atan2(-dir.X, -dir.Z), pawn.Rotation.Z);
        }

        PhysicsHandler.ApplyGravity(pawn, delta);
        MovementHandler.MoveAndSlide(pawn);
    }
}
