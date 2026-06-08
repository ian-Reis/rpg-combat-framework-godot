using System.Diagnostics;
using Godot;
using Helpers;

namespace Components;

[GlobalClass]
public partial class InputRotateModelComponent : Node
{
    [Export] public Node3D Model { get; set; }
    [Export] public float RotationSmoothness = 10f;
    [Export] public float InputDeadZone = 0.1f;

    private SystemLogicComponents _systemLogicComponents;

    public override void _Ready()
    {
        _systemLogicComponents = GetParentOrNull<SystemLogicComponents>();
        Debug.Assert(_systemLogicComponents != null, "InputRotateModelComponent must be a child of SystemLogicComponents");
    }

    public override void _Process(double delta)
    {
        if (Model == null) return;

        Vector2 inputDir = MovementHelper.GetInputDirection();
        if (inputDir.Length() <= InputDeadZone) return;

        float targetAngle = Mathf.Atan2(inputDir.X, -inputDir.Y);
        float currentAngle = Model.Rotation.Y;
        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, RotationSmoothness * (float)delta);

        Model.Rotation = new Vector3(Model.Rotation.X, newAngle, Model.Rotation.Z);
    }
}