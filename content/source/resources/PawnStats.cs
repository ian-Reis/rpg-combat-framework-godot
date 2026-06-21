using Godot;

namespace RPGFramework.Resources;

[GlobalClass]
public partial class PawnStats : Resource
{
    [ExportGroup("Movement")]
    [Export] public float Speed { get; set; } = 5.0f;
    [Export] public float Acceleration { get; set; } = 15.0f;
    [Export] public float Friction { get; set; } = 10.0f;

    [ExportGroup("Jump & Gravity")]
    [Export] public float JumpForce { get; set; } = 5.0f;
    [Export] public float GravityScale { get; set; } = 1.0f;

    [ExportGroup("Physics")]
    [Export] public float PushForce { get; set; } = 5.0f;
}
