using Animation;
using Components;
using Godot;
using Helpers;

namespace Animation;

// Plays the death animation and stays there indefinitely.
// No exit — the entity is dead.
[GlobalClass]
public partial class AnimStateDeath : AnimationState
{
    public override void Enter(AnimationStateMachineComponent sm)
    {
        AnimationTreeHelper.Travel(sm, StateName);
    }
}
