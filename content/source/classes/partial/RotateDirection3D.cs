using Godot;

namespace RPGFramework.Core;

[GlobalClass]
public partial class RotateDirection3D : Node3D
{
    [Export] public SpringArm3D SpringArm { get; set; }
    [Export] public float RotationSpeed { get; set; } = 10f;

    public override void _PhysicsProcess(double delta)
    {
        if (SpringArm == null)
            return;

        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        if (inputDir == Vector2.Zero)
            return;

        float yawRad = Mathf.DegToRad(SpringArm.RotationDegrees.Y);
        Vector3 forward = new(-Mathf.Sin(yawRad), 0f, -Mathf.Cos(yawRad));
        Vector3 right   = new( Mathf.Cos(yawRad), 0f, -Mathf.Sin(yawRad));
        Vector3 moveDir = (forward * inputDir.Y - right * inputDir.X).Normalized();

        float targetYawDeg = Mathf.RadToDeg(Mathf.Atan2(moveDir.X, moveDir.Z));
        float angleDelta   = Mathf.Wrap(targetYawDeg - RotationDegrees.Y, -180f, 180f);
        float t            = 1f - Mathf.Exp(-RotationSpeed * (float)delta);

        Vector3 rot = RotationDegrees;
        rot.Y += angleDelta * t;
        RotationDegrees = rot;
    }
}
