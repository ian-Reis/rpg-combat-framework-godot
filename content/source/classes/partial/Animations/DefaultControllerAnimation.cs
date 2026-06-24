using Godot;
using System.Dynamic;
using System.Collections.Generic;
using RPGFramework.Entitys;
using RPGFramework.Resources;

namespace RPGFramework.Core;

[GlobalClass]
public partial class DefaultControllerAnimation : Node
{
    [ExportGroup("References")]
    [Export] public Pawn3D Pawn            { get; set; }
    [Export] public AnimationTree AnimTree { get; set; }
    [Export] public PawnStats Stats        { get; set; }
    [Export] public HealthComponent Health { get; set; } // opcional: dispara a morte ao zerar

    [ExportGroup("AnimTree Params")]
    [Export] public Godot.Collections.Dictionary<StringName, string> BlendTo = new Godot.Collections.Dictionary<StringName, string>();
    [Export] public Godot.Collections.Dictionary<StringName, string> BlendSpace1DPosition = new Godot.Collections.Dictionary<StringName, string>();
    [Export] public Godot.Collections.Dictionary<StringName, string> Playback = new Godot.Collections.Dictionary<StringName, string>();
    [Export] public Godot.Collections.Dictionary<StringName, string> TransitionRequest = new Godot.Collections.Dictionary<StringName, string>();
    [Export] public Godot.Collections.Dictionary<StringName, string> OneShotRequest = new Godot.Collections.Dictionary<StringName, string>();
    
    // [Export] public string LocomotionBlendParam { get; set; } =
        // "parameters/DefaultBT/MovementSM/Locomotion/blend_position";
    // [Export] public string MovementSMParam { get; set; } =
        // "parameters/DefaultBT/MovementSM/playback";
    // [Export] public string JumpBlendParam { get; set; } = "parameters/DefaultBT/JumpBlend/blend_amount";
    // [Export] public string CrouchBlendParam { get; set; } = "parameters/DefaultBT/CrouchBlend/blend_amount";
    // [Export] public string CrouchBSParam    { get; set; } = "parameters/DefaultBT/CrouchBS/blend_position";
    // [Export] public string ArmedBlendParam  { get; set; } = "parameters/DefaultBT/ArmedBlend/blend_amount";
    // [Export] public string CombatOneShotParam { get; set; } = "parameters/DefaultBT/CombatOneShot/request";
    // [Export] public string CombatSMParam    { get; set; } = "parameters/DefaultBT/CombatSM/playback";
    // [Export] public string HeavyOneShotParam { get; set; } = "parameters/DefaultBT/OneShot/request";
    // Transition no topo: estado "Alive" (tudo) vs "Dead" (animação de morte, segura no fim).
    // [Export] public string DeathTransitionParam { get; set; } = "parameters/AliveDead/transition_request";
    // [Export] public StringName DeathState { get; set; } = "Dead";

    [ExportGroup("Settings")]
    [Export] public float JumpBlendSpeed   { get; set; } = 10f;  // suavidade do blend locomotion↔Jump
    [Export] public float CrouchBlendSpeed { get; set; } = 10f;  // suavidade do blend em pé↔agachado
    [Export] public float ArmedBlendSpeed  { get; set; } = 8f;   // suavidade do blend desarmado↔armado

    private AnimationNodeStateMachinePlayback _movementPB;
    private AnimationNodeStateMachinePlayback _combatPB;
    
    private bool _dead;

    public override void _Ready()
    {
        Stats ??= Pawn?.Stats;
        Setup();
    }

    public void Setup()
    {
        if (AnimTree == null) { GD.PrintErr("[AnimController] AnimTree não atribuído!"); return; }
        if (Pawn == null)     { GD.PrintErr("[AnimController] Pawn não atribuído!"); return; }

        AnimTree.Active = true;
        AnimTree.CallbackModeProcess = AnimationMixer.AnimationCallbackModeProcess.Physics;

        _movementPB           = (AnimationNodeStateMachinePlayback)AnimTree.Get(Playback.GetValueOrDefault("movement"));
        _combatPB = (AnimationNodeStateMachinePlayback)AnimTree.Get(Playback.GetValueOrDefault("combat"));

        _movementPB?.Travel("Locomotion");

        AnimTree.AnimationFinished += HandleAnimationFinished;
        SetupCombatCallbacks();

        if (Health != null)
            Health.Died += Die;
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
    }

    private float GetHorizontalSpeed() => new Vector2(Pawn.Velocity.X, Pawn.Velocity.Z).Length();
}
