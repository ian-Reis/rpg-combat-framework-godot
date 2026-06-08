using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private bool _isReady = false;

    public override void _Ready()
    {
        base._Ready();
        var context = GetParent<ISystemLogicContext>();
        if (context == null)
        {
            GD.PrintErr("[StateMachineComponent] Parent must implement ISystemLogicContext");
            return;
        }

        _ = SetupAsync(context);
    }

    private async Task SetupAsync(ISystemLogicContext context)
    {
        try
        {
            _context = context;

            if (States == null || States.Length == 0)
            {
                GD.PrintErr("[StateMachineComponent] No states assigned!");
                return;
            }

            if (InitialState == null)
            {
                GD.PrintErr("[StateMachineComponent] InitialState not assigned!");
                return;
            }

            foreach (var state in States)
            {
                if (state == null) continue;
                _statesMap[state.StateName] = state;
            }

            await ToSignal(_context as Node, Node.SignalName.Ready);

            ChangeState(InitialState.StateName);
            _isReady = true;
        }
        catch (System.Exception e)
        {
            GD.PrintErr($"[StateMachineComponent] Setup failed: {e.Message}\n{e.StackTrace}");
        }
    }

    public void ChangeState(string newStateName)
    {
        if (_context == null)
        {
            GD.PrintErr("[StateMachineComponent] Context is null, cannot change state");
            return;
        }

        if (string.IsNullOrEmpty(newStateName))
        {
            GD.PrintErr("[StateMachineComponent] State name cannot be null or empty");
            return;
        }

        if (!_statesMap.TryGetValue(newStateName, out var newState))
        {
            GD.PrintErr($"[StateMachineComponent] State not found: '{newStateName}'");
            return;
        }

        _currentState?.Exit(_context);
        _currentState = newState;
        GD.Print($"[StateMachine] → {newStateName}");
        _currentState.Enter(_context);
    }

    public override void _Process(double delta)
    {
        if (!_isReady || _currentState == null) return;
        _currentState.Update(_context, (float)delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_isReady || _currentState == null) return;
        _currentState.PhysicsUpdate(_context, (float)delta);
    }

    public override void _Input(InputEvent @event)
    {
        if (!_isReady || _currentState == null) return;
        _currentState.HandleInput(_context, @event);
    }
}
