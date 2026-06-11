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

    public override void Enter(AnimationStateMachineComponent sm)
    {
        AnimationTreeHelper.SetTreeCondition(sm, AnimationParams.IsIdle, true);
    }

    public override void Update(AnimationStateMachineComponent sm, AnimationSnapshot snapshot, float delta)
    {
        // Blend suave baseado na velocidade real, não em booleans
        sm.AnimationTree?.Set(BlendPath, snapshot.NormalizedSpeed);

        if (!snapshot.IsGrounded)
            sm.ChangeState("airborne");
    }

    public override void Exit(AnimationStateMachineComponent sm)
    {
        AnimationTreeHelper.SetTreeCondition(sm, AnimationParams.IsIdle, false);
    }
}
