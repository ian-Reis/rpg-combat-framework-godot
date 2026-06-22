using Godot;
using RPGFramework.Entitys;
using RPGFramework.Resources;

namespace RPGFramework.Core;

[GlobalClass]
public partial class DefaultControllerAnimation : Node
{
    [ExportGroup("References")]
    [Export] public AnimationTree AnimTree { get; set; }
    [Export] public Pawn3D Pawn            { get; set; }
    [Export] public PawnStats Stats        { get; set; }

    [ExportGroup("AnimTree Params")]
    [Export] public string LocomotionBlendParam { get; set; } =
        "parameters/DefaultBT/LocomotionSM/Locomotion/blend_position";
    [Export] public string StateMachineParam { get; set; } =
        "parameters/DefaultBT/LocomotionSM/playback";
    [Export] public string JumpBlendParam   { get; set; } = "parameters/DefaultBT/JumpBlend/blend_amount";
    [Export] public string JumpSMParam      { get; set; } = "parameters/DefaultBT/JumpSM/playback";
    [Export] public string CombatBlendParam { get; set; } = "parameters/DefaultBT/CombatBlend/blend_amount";
    [Export] public string CombatSMParam    { get; set; } = "parameters/DefaultBT/CombatSM/playback";

    [ExportGroup("Settings")]
    [Export] public float JumpBlendSpeed   { get; set; } = 10f;
    [Export] public float LandBlendSpeed   { get; set; } = 3f;
    [Export] public float CombatBlendSpeed { get; set; } = 8f;

    private AnimationNodeStateMachinePlayback _playback;
    private AnimationNodeStateMachinePlayback _jumpSMPlayback;
    private AnimationNodeStateMachinePlayback _combatSMPlayback;

    // Chamado por MeshAnimation._Ready() após injetar Pawn e Stats
    public void Setup()
    {
        if (AnimTree == null) { GD.PrintErr("[AnimController] AnimTree não atribuído!"); return; }
        if (Pawn == null)     { GD.PrintErr("[AnimController] Pawn não atribuído!"); return; }

        AnimTree.Active = true;
        AnimTree.CallbackModeProcess = AnimationMixer.AnimationCallbackModeProcess.Physics;
        _playback         = (AnimationNodeStateMachinePlayback)AnimTree.Get(StateMachineParam);
        _jumpSMPlayback   = (AnimationNodeStateMachinePlayback)AnimTree.Get(JumpSMParam);
        _combatSMPlayback = (AnimationNodeStateMachinePlayback)AnimTree.Get(CombatSMParam);

        _playback?.Travel("Locomotion");

        AnimTree.AnimationFinished += HandleAnimationFinished;
        SetupCombatCallbacks();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (AnimTree == null || Pawn == null)
            return;

        Vector3 vel = Pawn.Velocity;
        float dt = (float)delta;
        float horizontalSpeed = new Vector2(vel.X, vel.Z).Length();

        UpdateLocomotion(horizontalSpeed);
        UpdateJump(dt);
        UpdateCombat(dt);
    }
}
