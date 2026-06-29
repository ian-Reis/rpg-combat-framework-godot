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
    private AnimationNodeStateMachinePlayback _slidePB;
    
    private bool _dead;

    private StringName _logName = $"AnimationController";

    public override void _Ready()
    {
        if (AnimTree is null || Pawn is null) { GD.PrintErr("[AnimController] AnimTree ou Pawn não atribuído!"); return; }
        ConnectSignals();

        AnimTree.Active = true;
        AnimTree.CallbackModeProcess = AnimationMixer.AnimationCallbackModeProcess.Physics;

        TryGetPlaybacks();

        PlayInitalAnimation(_movementPB, "Locomotion");

        CombatCallbacks();
        SlideCallbacks();
        
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
        UpdateCombat(dt);
        UpdatePistol(dt);
        UpdateSlide(dt, horizontalSpeed);
        UpdateTreeChapping(dt);
        UpdateRoll(dt);
    }

    private void TryGetPlaybacks()
    {
        _movementPB = (AnimationNodeStateMachinePlayback)AnimTree.Get(Parameters?.Playback?.GetValueOrDefault("movement"));
        _combatPB   = (AnimationNodeStateMachinePlayback)AnimTree.Get(Parameters?.Playback?.GetValueOrDefault("combat"));
        _slidePB    = (AnimationNodeStateMachinePlayback)AnimTree.Get(Parameters?.Playback?.GetValueOrDefault("slide"));
    }

    private void PlayInitalAnimation(AnimationNodeStateMachinePlayback playback, string animName) => playback?.Travel(animName);
    
    private void ConnectSignals()
    {
        int succ = 0;
        succ += AnimTree.TryConnect(() => AnimTree.AnimationFinished += HandleAnimationFinished, _logName);
        succ += Health.TryConnect(() => Health.Died += Die, _logName);
    
        GD.Print($"{_logName} ConnectSignals Success {succ}/2");
    }

    private float GetHorizontalSpeed() => new Vector2(Pawn.Velocity.X, Pawn.Velocity.Z).Length();
}
