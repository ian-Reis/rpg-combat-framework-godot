using Components;
using Godot;
using Interfaces;

namespace Resources.states;

[GlobalClass]
public partial class State : Resource
{
    [Export] public string StateName = "";

    public virtual void Enter(StateMachineComponent stateMachineComponent) { }

    public virtual void Exit(StateMachineComponent stateMachineComponent) { }

    public virtual void Update(StateMachineComponent stateMachineComponent, float delta) { }

    public virtual void PhysicsUpdate(StateMachineComponent stateMachineComponent, float delta) { }

    public virtual void HandleInput(StateMachineComponent stateMachineComponent, InputEvent @event) { }
}
