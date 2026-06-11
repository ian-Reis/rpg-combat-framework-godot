using Components;
using Godot;
using Interfaces;

namespace Resources.states;

[GlobalClass]
public partial class LogicState : Resource
{
    [Export] public string StateName = "";

    public virtual void Enter(LogicStateMachineComponent stateMachineComponent) { }

    public virtual void Exit(LogicStateMachineComponent stateMachineComponent) { }

    public virtual void Update(LogicStateMachineComponent stateMachineComponent, float delta) { }

    public virtual void PhysicsUpdate(LogicStateMachineComponent stateMachineComponent, float delta) { }

    public virtual void HandleInput(LogicStateMachineComponent stateMachineComponent, InputEvent @event) { }
}
