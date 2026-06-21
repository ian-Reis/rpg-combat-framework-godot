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
    [Signal] public delegate void StateChangedEventHandler(string from, string to);

    [ExportGroup("References")]
    [Export] public AnimationTree AnimationTree { get; set; }
    [Export] public string        PlaybackPath  { get; set; } = "parameters/playback";
    [Export] public Node3D        Model         { get; set; }

    [ExportGroup("States")]
    [Export] public AnimationState   InitialState { get; set; }
    [Export] public AnimationState[] States       { get; set; }

    public AnimationSnapshot CurrentSnapshot { get; private set; }
    public string CurrentStateName { get; private set; } = "";

    private Pawn _pawn;
    private readonly Dictionary<string, AnimationState> _statesMap = new();
    private AnimationState _currentState;
    private float _timeInAir = 0f;
    private bool _isReady = false;

    public override void _Ready()
    {
        _pawn = GetParent<Pawn>();
        _ = SetupAsync();
    }

    private async Task SetupAsync()
    {
        try
        {
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

            await ToSignal(this, Node.SignalName.Ready);

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

        string previous = CurrentStateName;
        _currentState?.Exit(this);
        _currentState = newState;
        CurrentStateName = newStateName;
        GD.Print($"[AnimationSM] → {newStateName}");
        _currentState.Enter(this);
        EmitSignal(SignalName.StateChanged, previous, newStateName);
    }

    public override void _Process(double delta)
    {
        if (!_isReady || _currentState == null) return;

        float dt = (float)delta;
        CurrentSnapshot = BuildSnapshot(dt);
        _currentState.Update(this, CurrentSnapshot, dt);
    }

    private Basis GetMoveBasis()
    {
        if (_pawn == null) return Basis.Identity;
        Vector3 up = _pawn.UpDirection.Normalized();

        if (Model != null)
        {
            Vector3 mFwd   = -Model.GlobalTransform.Basis.Z;
            Vector3 mRight =  Model.GlobalTransform.Basis.X;

            Vector3 forward = (mFwd   - up * mFwd.Dot(up)).Normalized();
            Vector3 right   = (mRight - up * mRight.Dot(up)).Normalized();
            Vector3 newUp   = right.Cross(forward).Normalized();

            return new Basis(right, newUp, forward);
        }

        SpringArm3D springArm = _pawn.Camera?.SpringArm;
        if (springArm == null)
            return _pawn.GlobalTransform.Basis;

        Vector3 camFwd   = -springArm.GlobalTransform.Basis.Z;
        Vector3 camRight =  springArm.GlobalTransform.Basis.X;

        Vector3 fwdFlat   = (camFwd   - up * camFwd.Dot(up)).Normalized();
        Vector3 rightFlat = (camRight - up * camRight.Dot(up)).Normalized();
        Vector3 upFlat    = rightFlat.Cross(fwdFlat).Normalized();

        return new Basis(rightFlat, upFlat, -fwdFlat);
    }

    private AnimationSnapshot BuildSnapshot(float delta)
    {
        if (_pawn == null) return default;

        bool  isGrounded       = CharacterBodyHelper.IsOnFloor(_pawn);
        float horizontalSpeed  = CharacterBodyHelper.GetHorizontalSpeed(_pawn);
        float verticalVelocity = _pawn.Velocity.Y;

        _timeInAir = isGrounded ? 0f : _timeInAir + delta;

        float runSpeed        = _pawn.Stats?.RunSpeed ?? 6f;
        float normalizedSpeed = runSpeed > 0f ? Mathf.Clamp(horizontalSpeed / runSpeed, 0f, 1f) : 0f;

        return new AnimationSnapshot
        {
            HorizontalSpeed    = horizontalSpeed,
            NormalizedSpeed    = normalizedSpeed,
            VerticalVelocity   = verticalVelocity,
            IsGrounded         = isGrounded,
            HasInput           = Helpers.InputHelper.GetInputDirection().Length() > 0.1f,
            TimeInAir          = _timeInAir,
            RootMotionVelocity = Helpers.AnimationTreeHelper.GetRootMotionVelocity(this, GetMoveBasis(), delta),
        };
    }
}
