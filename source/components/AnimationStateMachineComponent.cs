using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Animation;
using Godot;
using Helpers;
using Interfaces;

namespace Components;

[GlobalClass]
public partial class AnimationStateMachineComponent : Node, IHasAnimationTree
{
    [ExportGroup("References")]
    [Export] public AnimationTree AnimationTree { get; set; }

    [ExportGroup("States")]
    [Export] public AnimationState InitialState { get; set; }
    [Export] public AnimationState[] States { get; set; }

    public ISystemLogicContext Context { get; private set; }
    public AnimationSnapshot CurrentSnapshot { get; private set; }

    private readonly Dictionary<string, AnimationState> _statesMap = new();
    private AnimationState _currentState;
    private float _timeInAir = 0f;
    private bool _isReady = false;

    public override void _Ready()
    {
        _ = SetupAsync();
    }

    private async Task SetupAsync()
    {
        try
        {
            Context = GetParent<ISystemLogicContext>();
            if (Context == null)
            {
                GD.PrintErr("[AnimationStateMachineComponent] Parent must implement ISystemLogicContext");
                return;
            }

            if (States == null || States.Length == 0)
            {
                GD.PrintErr("[AnimationStateMachineComponent] No states assigned!");
                return;
            }

            if (InitialState == null)
            {
                GD.PrintErr("[AnimationStateMachineComponent] InitialState not assigned!");
                return;
            }

            foreach (var state in States)
            {
                if (state == null) continue;
                _statesMap[state.StateName] = state;
            }

            await ToSignal(Context as Node, Node.SignalName.Ready);

            ChangeState(InitialState.StateName);
            _isReady = true;
        }
        catch (Exception e)
        {
            GD.PrintErr($"[AnimationStateMachineComponent] Setup failed: {e.Message}\n{e.StackTrace}");
        }
    }

    public void ChangeState(string newStateName)
    {
        if (string.IsNullOrEmpty(newStateName)) return;

        if (!_statesMap.TryGetValue(newStateName, out var newState))
        {
            GD.PrintErr($"[AnimationStateMachineComponent] State not found: '{newStateName}'");
            return;
        }

        _currentState?.Exit(this);
        _currentState = newState;
        GD.Print($"[AnimationSM] → {newStateName}");
        _currentState.Enter(this);
    }

    public override void _Process(double delta)
    {
        if (!_isReady || _currentState == null) return;

        float dt = (float)delta;
        CurrentSnapshot = BuildSnapshot(dt);
        _currentState.Update(this, CurrentSnapshot, dt);
    }

    private AnimationSnapshot BuildSnapshot(float delta)
    {
        bool isGrounded;
        float horizontalSpeed;
        float verticalVelocity;

        switch (Context.Pawn)
        {
            case CharacterBody3D charBody:
                isGrounded      = CharacterBodyHelper.IsOnFloor(charBody);
                horizontalSpeed = CharacterBodyHelper.GetHorizontalSpeed(charBody);
                verticalVelocity = charBody.Velocity.Y;
                break;
            case RigidBody3D rigidBody:
                isGrounded       = RigidBodyHelper.IsGrounded(rigidBody);
                horizontalSpeed  = RigidBodyHelper.GetHorizontalSpeed(rigidBody);
                verticalVelocity = rigidBody.LinearVelocity.Y;
                break;
            default:
                return default;
        }

        _timeInAir = isGrounded ? 0f : _timeInAir + delta;

        float runSpeed      = Context.Stats?.RunSpeed ?? 6f;
        float normalizedSpeed = runSpeed > 0f ? Mathf.Clamp(horizontalSpeed / runSpeed, 0f, 1f) : 0f;

        return new AnimationSnapshot
        {
            HorizontalSpeed  = horizontalSpeed,
            NormalizedSpeed  = normalizedSpeed,
            VerticalVelocity = verticalVelocity,
            IsGrounded       = isGrounded,
            HasInput         = Helpers.InputHelper.GetInputDirection().Length() > 0.1f,
            TimeInAir        = _timeInAir,
        };
    }
}
