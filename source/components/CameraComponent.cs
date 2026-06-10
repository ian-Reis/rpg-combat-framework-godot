using Godot;

namespace Components;

[GlobalClass]
public partial class CameraComponent : Node
{
    [ExportGroup("References")]
    [Export] public Camera3D Camera { get; set; }
    [Export] public SpringArm3D SpringArm { get; set; }

    [ExportGroup("Mouse")]
    [Export] public float MouseSensitivity = 0.3f;
    [Export] public bool CaptureMouseOnReady = true;

    [ExportGroup("Vertical Angles")]
    [Export] public float MaxVerticalAngle = 80f;
    [Export] public float MinVerticalAngle = -30f;

    [ExportGroup("Zoom")]
    [Export] public float MinZoom = 2f;
    [Export] public float MaxZoom = 10f;
    [Export] public float ZoomSpeed = 1f;
    [Export] public float ZoomSmoothness = 10f;

    [ExportGroup("Rotation")]
    [Export] public float RotationSmoothness = 15f;

    private float _pitch = 0f;
    private float _yaw = 0f;
    private float _targetPitch = 0f;
    private float _targetYaw = 0f;
    private float _targetZoom;

    public override void _Ready()
    {
        if (SpringArm == null)
        {
            GD.PrintErr("[CameraComponent] SpringArm not assigned!");
            return;
        }

        _targetZoom = MinZoom;
        SpringArm.SpringLength = MinZoom;

        if (Camera != null)
            Camera.Current = true;

        if (CaptureMouseOnReady)
            Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
            HandleMouseLook(mouseMotion);

        if (@event is InputEventMouseButton mouseButton && Input.MouseMode == Input.MouseModeEnum.Captured)
            HandleZoom(mouseButton);

        if (@event.IsActionPressed("ui_cancel"))
            Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured
                ? Input.MouseModeEnum.Visible
                : Input.MouseModeEnum.Captured;
    }

    public override void _Process(double delta)
    {
        if (SpringArm == null) return;

        float dt = (float)delta;

        _pitch = Mathf.Lerp(_pitch, _targetPitch, RotationSmoothness * dt);
        _yaw   = Mathf.Lerp(_yaw,   _targetYaw,   RotationSmoothness * dt);

        SpringArm.Rotation = new Vector3(
            Mathf.DegToRad(_pitch),
            Mathf.DegToRad(_yaw),
            0f
        );

        SpringArm.SpringLength = Mathf.Lerp(SpringArm.SpringLength, _targetZoom, ZoomSmoothness * dt);
    }

    private void HandleMouseLook(InputEventMouseMotion mouseMotion)
    {
        _targetYaw   -= mouseMotion.Relative.X * MouseSensitivity;
        _targetPitch -= mouseMotion.Relative.Y * MouseSensitivity;
        _targetPitch  = Mathf.Clamp(_targetPitch, MinVerticalAngle, MaxVerticalAngle);
    }

    private void HandleZoom(InputEventMouseButton mouseButton)
    {
        if (mouseButton.ButtonIndex == MouseButton.WheelUp)
            _targetZoom = Mathf.Max(MinZoom, _targetZoom - ZoomSpeed);
        else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
            _targetZoom = Mathf.Min(MaxZoom, _targetZoom + ZoomSpeed);
    }
}
