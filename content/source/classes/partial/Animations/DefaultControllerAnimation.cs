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
    [Export] public HealthComponent Health { get; set; } // opcional: dispara a morte ao zerar

    [ExportGroup("AnimTree Params")]
    [Export] public string LocomotionBlendParam { get; set; } =
        "parameters/DefaultBT/MovementSM/Locomotion/blend_position";
    [Export] public string MovementSMParam { get; set; } =
        "parameters/DefaultBT/MovementSM/playback";
    [Export] public string JumpBlendParam { get; set; } = "parameters/DefaultBT/JumpBlend/blend_amount";
    [Export] public string CrouchBlendParam { get; set; } = "parameters/DefaultBT/CrouchBlend/blend_amount";
    [Export] public string CrouchBSParam    { get; set; } = "parameters/DefaultBT/CrouchBS/blend_position";
    [Export] public string ArmedBlendParam  { get; set; } = "parameters/DefaultBT/ArmedBlend/blend_amount";
    [Export] public string CombatOneShotParam { get; set; } = "parameters/DefaultBT/CombatOneShot/request";
    [Export] public string CombatSMParam    { get; set; } = "parameters/DefaultBT/CombatSM/playback";
    [Export] public string HeavyOneShotParam { get; set; } = "parameters/DefaultBT/OneShot/request";
    // Transition no topo: estado "Alive" (tudo) vs "Dead" (animação de morte, segura no fim).
    [Export] public string DeathTransitionParam { get; set; } = "parameters/AliveDead/transition_request";
    [Export] public StringName DeathState { get; set; } = "Dead";

    [ExportGroup("Settings")]
    [Export] public float JumpBlendSpeed   { get; set; } = 10f;  // suavidade do blend locomotion↔Jump
    [Export] public float CrouchBlendSpeed { get; set; } = 10f;  // suavidade do blend em pé↔agachado
    [Export] public float ArmedBlendSpeed  { get; set; } = 8f;   // suavidade do blend desarmado↔armado

    private AnimationNodeStateMachinePlayback _moveSM;
    private AnimationNodeStateMachinePlayback _combatSMPlayback;
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
        _moveSM           = (AnimationNodeStateMachinePlayback)AnimTree.Get(MovementSMParam);
        _combatSMPlayback = (AnimationNodeStateMachinePlayback)AnimTree.Get(CombatSMParam);

        _moveSM?.Travel("Locomotion");

        AnimTree.AnimationFinished += HandleAnimationFinished;
        SetupCombatCallbacks();

        if (Health != null)
            Health.Died += Die;
    }

    // Troca pro estado "Dead" (Transition no topo) e congela o resto da animação.
    public void Die()
    {
        if (_dead) return;
        _dead = true;
        HitBox?.DisableDeferred(); // inimigo morto não dá mais dano (mesmo se morreu no meio do golpe)
        Pawn?.State?.SetAction(PawnState.Action.Dead);
        AnimTree.Set(DeathTransitionParam, DeathState);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (AnimTree == null || Pawn == null || _dead)
            return;

        Vector3 vel = Pawn.Velocity;
        float dt = (float)delta;
        float horizontalSpeed = new Vector2(vel.X, vel.Z).Length();

        UpdateLocomotion(horizontalSpeed);
        UpdateArmed(dt);
        UpdateCrouch(dt, horizontalSpeed);
        UpdateJump(dt);
        UpdateCombat();
    }
}
