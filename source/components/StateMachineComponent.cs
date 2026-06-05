using Godot;
using System.Collections.Generic;
using Resources.states;

// ReSharper disable once CheckNamespace
namespace Components;

[GlobalClass]
public partial class StateMachineComponent : Node
{
    [ExportGroup("States")]
    [Export] public State InitialState { get; set; }
    [Export] public State[] States { get; set; }

    private SystemLogicComponents _systemLogicComponents;
    private State _currentState;
    private readonly Dictionary<string, State> _statesMap = new();

    public override void _Ready()
    {
        base._Ready();
        Setup(GetParent<SystemLogicComponents>());
    }

    private async void Setup(SystemLogicComponents systemLogicComponents)
    {
        try
        {
            _systemLogicComponents = systemLogicComponents;

            foreach (var state in States)
                _statesMap[state.StateName] = state;

            await ToSignal(_systemLogicComponents, Node.SignalName.Ready);

            ChangeState(InitialState.StateName);
        }
        catch (System.Exception e)
        {
            GD.PrintErr($"[StateMachineComponent] Setup failed: {e.Message}");
        }
    }

    public void ChangeState(string newStateName)
    {
        _currentState?.Exit(_systemLogicComponents);

        if (_statesMap.TryGetValue(newStateName, out var newState))
        {
            GD.Print($"[StateMachine] → {newStateName}");
            _currentState = newState;
            _currentState.Enter(_systemLogicComponents);
        }
        else
        {
            GD.PrintErr($"[StateMachine] State not found: '{newStateName}'");
        }
    }

    public override void _Process(double delta)
        => _currentState?.Update(_systemLogicComponents, (float)delta);

    public override void _PhysicsProcess(double delta)
        => _currentState?.PhysicsUpdate(_systemLogicComponents, (float)delta);

    public override void _Input(InputEvent @event)
        => _currentState?.HandleInput(_systemLogicComponents, @event);
}
