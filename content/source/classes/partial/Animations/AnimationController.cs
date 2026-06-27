using Godot;
using System.Dynamic;
using System.Collections.Generic;
using RPGFramework.Entitys;
using RPGFramework.Resources;
using RPGFramework.Helpers;

namespace RPGFramework.Core;

[GlobalClass]
public partial class AnimationController : Node
{
    [ExportGroup("References")]
    // [Export] public Godot.Collections.Dictionary<StringName, Node> Refs = new Godot.Collections.Dictionary<StringName, Node>();
    [Export] public Pawn3D Pawn            { get; set; }
    [Export] public AnimationTree AnimTree { get; set; }
    [ExportSubgroup("Opcionals")]
    [Export] public HealthComponent Health { get; set; } // opcional: dispara a morte ao zerar
    [Export] public WeaponComponent Weapon { get; set; } // arma equipada (dano por golpe)

    [ExportGroup("Animation Tree")]
    [Export] public AnimationTreeParameters Parameters { get; set; }

    private AnimationNodeStateMachinePlayback _movementPB;
    private AnimationNodeStateMachinePlayback _combatPB;
    
    private bool _dead;

    private StringName _logName = $"AnimationController";

    public override void _Ready()
    {
        Setup();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (AnimTree == null || Pawn == null || _dead)
            return;

        float dt = (float)delta;
        float horizontalSpeed = GetHorizontalSpeed();

        UpdateLocomotion(horizontalSpeed);
        UpdateArmed(dt);
        UpdateCrouch(dt, horizontalSpeed);
        UpdateJump(dt);
        UpdateCombat();
        UpdatePistol(dt);
    }

    public void Setup()
    {
        if (AnimTree == null) { GD.PrintErr("[AnimController] AnimTree não atribuído!"); return; }
        if (Pawn == null)     { GD.PrintErr("[AnimController] Pawn não atribuído!");     return; }

        AnimTree.Active = true;
        AnimTree.CallbackModeProcess = AnimationMixer.AnimationCallbackModeProcess.Physics;

        _movementPB = (AnimationNodeStateMachinePlayback)AnimTree.Get(Parameters?.Playback?.GetValueOrDefault("movement"));
        _combatPB   = (AnimationNodeStateMachinePlayback)AnimTree.Get(Parameters?.Playback?.GetValueOrDefault("combat"));

        _movementPB?.Travel("Locomotion");

        SetupCombatCallbacks();
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        int succ = 0;
        succ += AnimTree.TryConnect(() => AnimTree.AnimationFinished += HandleAnimationFinished, _logName);
        succ += Health.TryConnect(() => Health.Died += Die, _logName);
    
        GD.Print($"{_logName} ConnectSignals Success {succ}/2");
    }

    private float GetHorizontalSpeed() => new Vector2(Pawn.Velocity.X, Pawn.Velocity.Z).Length();
}
