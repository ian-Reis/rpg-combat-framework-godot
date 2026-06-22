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
        "parameters/DefaultBT/MovementSM/Locomotion/blend_position";
    [Export] public string MovementSMParam { get; set; } =
        "parameters/DefaultBT/MovementSM/playback";
    [Export] public string JumpBlendParam { get; set; } = "parameters/DefaultBT/JumpBlend/blend_amount";
    [Export] public string CrouchBlendParam { get; set; } = "parameters/DefaultBT/CrouchBlend/blend_amount";
    [Export] public string CrouchBSParam    { get; set; } = "parameters/DefaultBT/CrouchBS/blend_position";
    [Export] public string CombatOneShotParam { get; set; } = "parameters/DefaultBT/CombatOneShot/request";
    [Export] public string CombatSMParam    { get; set; } = "parameters/DefaultBT/CombatSM/playback";
    [Export] public string HeavyOneShotParam { get; set; } = "parameters/DefaultBT/OneShot/request";

    [ExportGroup("Settings")]
    [Export] public float JumpBlendSpeed   { get; set; } = 10f;  // suavidade do blend locomotion↔Jump
    [Export] public float CrouchBlendSpeed { get; set; } = 10f;  // suavidade do blend em pé↔agachado

    private AnimationNodeStateMachinePlayback _moveSM;
    private AnimationNodeStateMachinePlayback _combatSMPlayback;

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
        _moveSM           = (AnimationNodeStateMachinePlayback)AnimTree.Get(MovementSMParam);
        _combatSMPlayback = (AnimationNodeStateMachinePlayback)AnimTree.Get(CombatSMParam);

        _moveSM?.Travel("Locomotion");

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
        UpdateCrouch(dt, horizontalSpeed);
        UpdateJump(dt);
        UpdateCombat();
    }
}
