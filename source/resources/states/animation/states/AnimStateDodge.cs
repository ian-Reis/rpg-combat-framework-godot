using Animation;
using Components;
using Godot;
using Helpers;

namespace Animation;

[GlobalClass]
public partial class AnimStateDodge : AnimationState
{
    public override void Enter(AnimationStateMachineComponent sm)
    {
        AnimationTreeHelper.Travel(sm, StateName);
    }

    public override void Update(AnimationStateMachineComponent sm, AnimationSnapshot snapshot, float delta)
    {
        // AnimationTree drives the "At End" transition to locomotion — just observe when it happened.
        if (AnimationTreeHelper.GetCurrentNode(sm) != StateName)
            sm.ChangeState("locomotion");
    }
}
