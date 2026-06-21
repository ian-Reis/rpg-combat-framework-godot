using Godot;
using Handlers;
using Constants;
using Components;

namespace Resources.states;

[GlobalClass]
public partial class LogicStateAIInvestigate : LogicState
{
    [Export] public float  ArrivalThreshold = 0.8f;
    [Export] public string DoneState        = LogicStateNames.AiIdle;

    public override void Enter(LogicStateMachineComponent sm)
    {
        sm.Pawn.AnimationSM?.ChangeState("locomotion");
    }

    public override void PhysicsUpdate(LogicStateMachineComponent sm, float delta)
    {
        var pawn = sm.Pawn;
        if (pawn == null) return;

        var memory = pawn.Memory;
        if (memory == null || !memory.HasMemory)
        {
            sm.ChangeState(DoneState);
            return;
        }

        Vector3 dir = memory.LastKnownPosition - pawn.GlobalPosition;
        dir.Y = 0f;

        if (dir.Length() <= ArrivalThreshold)
        {
            sm.ChangeState(DoneState);
            return;
        }

        dir = dir.Normalized();
        float speed = pawn.Stats?.WalkSpeed ?? 3f;
        pawn.Velocity = new Vector3(dir.X * speed, pawn.Velocity.Y, dir.Z * speed);
        pawn.Rotation = new Vector3(pawn.Rotation.X, Mathf.Atan2(-dir.X, -dir.Z), pawn.Rotation.Z);

        PhysicsHandler.ApplyGravity(pawn, delta);
        MovementHandler.MoveAndSlide(pawn);
    }
}
