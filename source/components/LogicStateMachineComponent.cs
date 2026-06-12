using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resources.states;
using Interfaces;

// ReSharper disable once CheckNamespace
namespace Components;

[GlobalClass]
public partial class LogicStateMachineComponent : Node
{
    [ExportGroup("States")]
    [Export] public LogicState InitialState { get; set; }
    [Export] public LogicState[] States { get; set; }

    public ISystemLogicContext systemLogicContext { get; private set; }
    public string CurrentStateName { get; private set; } = "";

    private readonly Dictionary<string, LogicState> _statesMap = new();
    
    private LogicState _currentState;
    private bool _isReady = false;

    public override void _Ready()
    {
        base._Ready();
        var context = GetParent<ISystemLogicContext>();
        if (context == null)
        {
            GD.PrintErr("[LogicStateMachineComponent] Parent must implement ISystemLogicContext");
            return;
        }

        _ = SetupAsync(context);
    }

    private async Task SetupAsync(ISystemLogicContext context)
    {
        try
        {
            systemLogicContext = context;

            if (States == null || States.Length == 0)
            {
                GD.PrintErr("[LogicStateMachineComponent] No states assigned!");
                return;
            }

            if (InitialState == null)
            {
                GD.PrintErr("[LogicStateMachineComponent] InitialState not assigned!");
                return;
            }

            foreach (var state in States)
            {
                if (state == null) continue;
                _statesMap[state.StateName] = state;
            }

            await ToSignal(systemLogicContext as Node, Node.SignalName.Ready);

            ChangeState(InitialState.StateName);
            _isReady = true;
        }
        catch (System.Exception e)
        {
            GD.PrintErr($"[LogicStateMachineComponent] Setup failed: {e.Message}\n{e.StackTrace}");
        }
    }

    public void ChangeState(string newStateName)
    {
        if (systemLogicContext == null)
        {
            GD.PrintErr("[LogicStateMachineComponent] Context is null, cannot change state");
            return;
        }

        if (string.IsNullOrEmpty(newStateName))
        {
            GD.PrintErr("[LogicStateMachineComponent] State name cannot be null or empty");
            return;
        }

        if (!_statesMap.TryGetValue(newStateName, out var newState))
        {
            GD.PrintErr($"[LogicStateMachineComponent] State not found: '{newStateName}'");
            return;
        }

        _currentState?.Exit(this);
        _currentState = newState;
        CurrentStateName = newStateName;
        GD.Print($"[LogicStateMachineComponent] → {newStateName}");
        _currentState.Enter(this);
    }

    public override void _Process(double delta)
    {
        if (!_isReady || _currentState == null) return;
        _currentState.Update(this, (float)delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_isReady || _currentState == null) return;
        _currentState.PhysicsUpdate(this, (float)delta);
    }

    public override void _Input(InputEvent @event)
    {
        if (!_isReady || _currentState == null) return;
        _currentState.HandleInput(this, @event);
    }
}
