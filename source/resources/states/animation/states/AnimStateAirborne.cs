using Components;
using Constants;
using Godot;
using Helpers;

namespace Animation;

[GlobalClass]
public partial class AnimStateAirborne : AnimationState
{
    // Path no blend tree de salto (-1=queda máxima, 0=apex, 1=subida máxima)
    [Export] public string BlendPath     = "parameters/jump/blend_position";
    [Export] public float  BlendMaxSpeed = 10f;

    public override void Enter(AnimationStateMachineComponent sm)
    {
        AnimationTreeHelper.SetTreeCondition(sm, AnimationParams.IsJump, true);
    }

    public override void Update(AnimationStateMachineComponent sm, AnimationSnapshot snapshot, float delta)
    {
        // Mapeia velocidade vertical para -1..1 para blend ascent/descent
        float blend = Mathf.Clamp(snapshot.VerticalVelocity / BlendMaxSpeed, -1f, 1f);
        sm.AnimationTree?.Set(BlendPath, blend);

        if (snapshot.IsGrounded)
            sm.ChangeState("land");
    }

    public override void Exit(AnimationStateMachineComponent sm)
    {
        AnimationTreeHelper.SetTreeCondition(sm, AnimationParams.IsJump, false);
    }
}
