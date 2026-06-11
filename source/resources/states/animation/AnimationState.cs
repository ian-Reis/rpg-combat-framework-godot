using Components;
using Godot;

namespace Animation;

[GlobalClass]
public partial class AnimationState : Resource
{
    [Export] public string StateName = "";

    public virtual void Enter(AnimationStateMachineComponent sm) { }
    public virtual void Exit(AnimationStateMachineComponent sm) { }
    public virtual void Update(AnimationStateMachineComponent sm, AnimationSnapshot snapshot, float delta) { }
}
