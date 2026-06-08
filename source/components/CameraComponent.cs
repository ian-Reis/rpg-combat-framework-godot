using System.Diagnostics;
using Godot;

namespace Components;

[GlobalClass]
public partial class CameraComponent : Node
{
    [Export] public Camera3D Camera { get; set; }
    [Export] public SpringArm3D SpringArm { get; set; }
    [Export] public Node3D FollowTarget { get; set; }
    [Export] public float MouseSensitivity = 0.3f;
    [Export] public float MaxVerticalAngle = 60f;
    [Export] public float MinVerticalAngle = -30f;
    [Export] public float FollowSmoothness = 10f;
    [Export] public float RotationSmoothness = 15f;
    [Export] public float MinZoom = 2f;
    [Export] public float MaxZoom = 10f;
    [Export] public float ZoomSpeed = 1f;

    private float _pitch = 0f;
    private float _yaw = 0f;
    private float _targetPitch = 0f;
    private float _targetYaw = 0f;
    private float _targetDistance;

    private SystemLogicComponents _systemLogicComponents;

    public override void _Ready()
    {
        _systemLogicComponents = GetParentOrNull<SystemLogicComponents>();
        Debug.Assert(_systemLogicComponents != null, "CameraComponent must be a child of SystemLogicComponents");

        FollowTarget ??= _systemLogicComponents?.Pawn as Node3D;

        if (SpringArm != null)
            SpringArm.SpringLength = MinZoom;

        if (Camera != null)
            Camera.Current = true;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
            HandleMouseInput(mouseMotion);

        if (@event is InputEventMouseButton mouseButton && Input.MouseMode == Input.MouseModeEnum.Captured)
            HandleZoom(mouseButton);

        if (@event.IsActionPressed("ui_cancel"))
            Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured
                ? Input.MouseModeEnum.Visible
                : Input.MouseModeEnum.Captured;
    }

    public override void _Process(double delta)
    {
        if (FollowTarget == null || SpringArm == null) return;

        float dt = (float)delta;

        _pitch = Mathf.Lerp(_pitch, _targetPitch, RotationSmoothness * dt);
        _yaw = Mathf.Lerp(_yaw, _targetYaw, RotationSmoothness * dt);

        SpringArm.GlobalPosition = SpringArm.GlobalPosition.Lerp(FollowTarget.GlobalPosition, FollowSmoothness * dt);

        SpringArm.Rotation = new Vector3(
            Mathf.DegToRad(_pitch),
            Mathf.DegToRad(_yaw),
            0
        );

        SpringArm.SpringLength = Mathf.Lerp(SpringArm.SpringLength, _targetDistance, FollowSmoothness * dt);
    }

    private void HandleMouseInput(InputEventMouseMotion mouseMotion)
    {
        _targetYaw -= mouseMotion.Relative.X * MouseSensitivity;
        _targetPitch -= mouseMotion.Relative.Y * MouseSensitivity;
        _targetPitch = Mathf.Clamp(_targetPitch, MinVerticalAngle, MaxVerticalAngle);
    }

    private void HandleZoom(InputEventMouseButton mouseButton)
    {
        if (mouseButton.ButtonIndex == MouseButton.WheelUp)
            _targetDistance = Mathf.Max(MinZoom, _targetDistance - ZoomSpeed);
        else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
            _targetDistance = Mathf.Min(MaxZoom, _targetDistance + ZoomSpeed);
    }
}