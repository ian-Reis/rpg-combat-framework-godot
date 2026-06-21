using Godot;
using Helpers;

namespace Components;

[GlobalClass]
public partial class InputRotateModelComponent : Node
{
    [ExportGroup("References")]
    [Export] public Node3D      Model     { get; set; }
    [Export] public SpringArm3D SpringArm { get; set; }

    [ExportGroup("Rotation")]
    [Export] public float RotationSmoothness = 10f;
    [Export] public float InputDeadZone      = 0.1f;
    [Export] public bool  InvertY            = false;

    [ExportGroup("Velocity Gate")]
    [Export] public bool  RequireVelocity     = true;
    [Export] public float MinVelocityToRotate = 0.1f;

    private Pawn _pawn;

    public override void _Ready()
    {
        _pawn     = GetParent<Pawn>();
        SpringArm ??= _pawn?.Camera?.SpringArm;
    }

    public override void _Process(double delta)
    {
        if (Model == null) return;

        Vector2 inputDir = InputHelper.GetInputDirection();
        if (inputDir.Length() <= InputDeadZone) return;

        if (RequireVelocity && !HasEnoughVelocity()) return;

        if (InvertY)
            inputDir.Y = -inputDir.Y;

        float targetAngle;
        if (SpringArm != null)
        {
            Vector3 camForward = -SpringArm.GlobalTransform.Basis.Z;
            Vector3 camRight   =  SpringArm.GlobalTransform.Basis.X;

            Vector3 moveDir = (camRight * inputDir.X + camForward * inputDir.Y).Normalized();
            targetAngle = Mathf.Atan2(moveDir.X, moveDir.Z);
        }
        else
        {
            targetAngle = Mathf.Atan2(inputDir.X, -inputDir.Y);
        }

        float currentAngle = Model.Rotation.Y;
        float newAngle     = Mathf.LerpAngle(currentAngle, targetAngle, RotationSmoothness * (float)delta);
        Model.Rotation     = new Vector3(Model.Rotation.X, newAngle, Model.Rotation.Z);
    }

    private bool HasEnoughVelocity()
    {
        if (_pawn == null) return true;
        Vector3 vel = _pawn.Velocity;
        return new Vector2(vel.X, vel.Z).Length() > MinVelocityToRotate;
    }
}
