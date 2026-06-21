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
        "parameters/DefaultBT/DefaultSM/Locomotion/blend_position";
    [Export] public string StateMachineParam { get; set; } =
        "parameters/DefaultBT/DefaultSM/travel";

    public override void _Ready()
    {
        if (AnimTree != null)
            AnimTree.Active = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (AnimTree == null || Pawn == null)
            return;

        Vector3 vel = Pawn.Velocity;
        float horizontalSpeed = new Vector2(vel.X, vel.Z).Length();

        UpdateLocomotion(horizontalSpeed);
    }
}
