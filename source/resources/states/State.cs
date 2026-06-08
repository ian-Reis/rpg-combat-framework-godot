using Godot;
using Interfaces;

namespace Resources.states;

[GlobalClass]
public partial class State : Resource
{
    [Export] public string StateName = "";

    public virtual void Enter(IPlayerStateContext context) { }

    public virtual void Exit(IPlayerStateContext context) { }

    public virtual void Update(IPlayerStateContext context, float delta) { }

    public virtual void PhysicsUpdate(IPlayerStateContext context, float delta) { }

    public virtual void HandleInput(IPlayerStateContext context, InputEvent @event) { }
}
