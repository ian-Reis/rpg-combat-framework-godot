using Godot;
using System.Collections.Generic;
using Resources.states;
using Interfaces;

// ReSharper disable once CheckNamespace
namespace Components;

[GlobalClass]
public partial class StateMachineComponent : Node
{
    [ExportGroup("States")]
    [Export] public State InitialState { get; set; }
    [Export] public State[] States { get; set; }

    private ISystemLogicContext _context;
    private State _currentState;
    private readonly Dictionary<string, State> _statesMap = new();

    public override void _Ready()
    {
        base._Ready();
        Setup(GetParent<ISystemLogicContext>());
    }

    private async void Setup(ISystemLogicContext context)
    {
        try
        {
            _context = context;

            foreach (var state in States)
                _statesMap[state.StateName] = state;

            await ToSignal(_context as Node, Node.SignalName.Ready);

            ChangeState(InitialState.StateName);
        }
        catch (System.Exception e)
        {
            GD.PrintErr($"[StateMachineComponent] Setup failed: {e.Message}");
        }
    }

    public void ChangeState(string newStateName)
    {
        _currentState?.Exit(_context);

        if (_statesMap.TryGetValue(newStateName, out var newState))
        {
            GD.Print($"[StateMachine] → {newStateName}");
            _currentState = newState;
            _currentState.Enter(_context);
        }
        else
        {
            GD.PrintErr($"[StateMachine] State not found: '{newStateName}'");
        }
    }

    public override void _Process(double delta)
        => _currentState?.Update(_context, (float)delta);

    public override void _PhysicsProcess(double delta)
        => _currentState?.PhysicsUpdate(_context, (float)delta);

    public override void _Input(InputEvent @event)
        => _currentState?.HandleInput(_context, @event);
}
