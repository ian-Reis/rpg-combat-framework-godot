using Components;
using Constants;
using Godot;
using Helpers;

namespace Animation;

[GlobalClass]
public partial class AnimStateLocomotion : AnimationState
{
    // Path no blend tree de locomotion (0=idle, 0.5=walk, 1=run)
    [Export] public string BlendPath = "parameters/locomotion/blend_position";

    [Export] public float SmoothSpeed = 8f;  // quão rápido acelera/desacelera

    private float _smoothSpeed = 0f;

    public override void Enter(AnimationStateMachineComponent sm)
    {
        AnimationTreeHelper.SetTreeCondition(sm, AnimationParams.IsIdle, true);
    }

    public override void Update(AnimationStateMachineComponent sm, AnimationSnapshot snapshot, float delta)
    {
        _smoothSpeed = Mathf.Lerp(_smoothSpeed, snapshot.NormalizedSpeed, SmoothSpeed * delta);
        // Blend suave baseado na velocidade real, não em booleans
        sm.AnimationTree?.Set(BlendPath, _smoothSpeed);

        if (!snapshot.IsGrounded)
            sm.ChangeState("airborne");
    }

    public override void Exit(AnimationStateMachineComponent sm)
    {
        AnimationTreeHelper.SetTreeCondition(sm, AnimationParams.IsIdle, false);
    }
}
