using Godot;

namespace Data;

[GlobalClass]
public partial class CharacterStats : Resource
{
    [ExportGroup("Movement")]
    [Export] public float WalkSpeed = 3f;
    [Export] public float RunSpeed  = 6f;

    [ExportGroup("Jump")]
    [Export] public float JumpForce     = 8f;
    [Export] public float JumpHoldTime  = 0.2f;
    [Export] public float CutJumpFactor = 0.5f;

    [ExportGroup("Physics")]
    [Export] public float Gravity       = 9.8f;
    [Export] public float AirResistance = 0f;
}
