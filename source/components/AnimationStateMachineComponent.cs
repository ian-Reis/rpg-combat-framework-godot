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
    [Export] public string PlaybackPath { get; set; } = "parameters/playback";

    [ExportGroup("States")]
    [Export] public AnimationState InitialState { get; set; }
    [Export] public AnimationState[] States { get; set; }

    public ISystemLogicContext Context { get; private set; }
    public AnimationSnapshot CurrentSnapshot { get; private set; }
    public string CurrentStateName { get; private set; } = "";

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
        CurrentStateName = newStateName;
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

    private Basis GetMoveBasis(CharacterBody3D charBody)
    {
        Vector3 up = charBody.UpDirection.Normalized();

        SpringArm3D springArm = Context.GetComponent<CameraComponent>()?.SpringArm;
        if (springArm == null)
            return charBody.GlobalTransform.Basis;

        Vector3 camForward = -springArm.GlobalTransform.Basis.Z;
        Vector3 camRight   =  springArm.GlobalTransform.Basis.X;

        Vector3 forward = (camForward - up * camForward.Dot(up)).Normalized();
        Vector3 right   = (camRight   - up * camRight.Dot(up)).Normalized();
        Vector3 newUp   = right.Cross(forward).Normalized();

        return new Basis(right, newUp, -forward);
    }

    private AnimationSnapshot BuildSnapshot(float delta)
    {
        if (Context.Pawn is not CharacterBody3D charBody) return default;

        bool  isGrounded     = CharacterBodyHelper.IsOnFloor(charBody);
        float horizontalSpeed = CharacterBodyHelper.GetHorizontalSpeed(charBody);
        float verticalVelocity = charBody.Velocity.Y;

        _timeInAir = isGrounded ? 0f : _timeInAir + delta;

        float runSpeed        = Context.Stats?.RunSpeed ?? 6f;
        float normalizedSpeed = runSpeed > 0f ? Mathf.Clamp(horizontalSpeed / runSpeed, 0f, 1f) : 0f;

        return new AnimationSnapshot
        {
            HorizontalSpeed    = horizontalSpeed,
            NormalizedSpeed    = normalizedSpeed,
            VerticalVelocity   = verticalVelocity,
            IsGrounded         = isGrounded,
            HasInput           = Helpers.InputHelper.GetInputDirection().Length() > 0.1f,
            TimeInAir          = _timeInAir,
            RootMotionVelocity = Helpers.AnimationTreeHelper.GetRootMotionVelocity(this, GetMoveBasis(charBody), delta),
        };
    }
}
