using System;
using System.Data.SqlTypes;
using System.Diagnostics;
using Godot;
using Helpers;

namespace Components;

[GlobalClass] public partial class InputRotateModelComponent : Node
{
    [Export] public Node3D Model { get; set; }
    private SystemLogicComponents _systemLogicComponents;

    public override void _EnterTree()
    {
        _systemLogicComponents = GetParentOrNull<SystemLogicComponents>();
        Debug.Assert(_systemLogicComponents != null, "InputRotateModelComponent must be a child of SystemLogicComponents");
    }

    public override void _Process(double delta)
    {
        if (Model is null) return;
        if (_systemLogicComponents?.Pawn is not CharacterBody3D pawn) return;

        var inputDir = MovementHelper.GetInputDirection();
        if (inputDir.Length() > 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDir.X, inputDir.Y);
            float currentAngle = Model.Rotation.Y;
            float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, 10f * (float)delta);
            Model.Rotation = new Vector3(Model.Rotation.X, newAngle, Model.Rotation.Z);

            // GD.Print($"InputDir: {inputDir}, TargetAngle: {Mathf.RadToDeg(targetAngle)}, CurrentAngle: {Mathf.RadToDeg(currentAngle)}, NewAngle: {Mathf.RadToDeg(newAngle)}");
        }
    }
}