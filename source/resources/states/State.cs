using Godot;
using Interfaces;

namespace Resources.states;

[GlobalClass]
public partial class State : Resource
{
    [Export] public string StateName = "";

    public virtual void Enter(ISystemLogicContext context) { }

    public virtual void Exit(ISystemLogicContext context) { }

    public virtual void Update(ISystemLogicContext context, float delta) { }

    public virtual void PhysicsUpdate(ISystemLogicContext context, float delta) { }

    public virtual void HandleInput(ISystemLogicContext context, InputEvent @event) { }
}
