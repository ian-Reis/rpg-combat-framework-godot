using Godot;
using Components;

namespace Resources.states;

[GlobalClass]
public partial class State : Resource
{
    [Export] public string StateName = "";

    public virtual void Enter(SystemLogicComponents owner) { }

    public virtual void Exit(SystemLogicComponents owner) { }

    public virtual void Update(SystemLogicComponents owner, float delta) { }

    public virtual void PhysicsUpdate(SystemLogicComponents owner, float delta) { }

    public virtual void HandleInput(SystemLogicComponents owner, InputEvent @event) { }
}
