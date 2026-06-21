using Godot;

namespace RPGFramework.Core;

[GlobalClass]
public partial class TPSSpringArm3D : SpringArm3D
{
    [ExportGroup("Rotation")]
    [Export] public float MouseSensitivity { get; set; } = 0.3f;
    [Export] public float MinPitch         { get; set; } = -89f;
    [Export] public float MaxPitch         { get; set; } =  89f;

    [ExportGroup("Smooth Follow")]
    [Export] public bool  UseSmoothFollow { get; set; } = true;
    [Export] public float SmoothSpeed     { get; set; } = 10f;

    [ExportGroup("Zoom")]
    [Export] public float ZoomStep      { get; set; } = 1f;
    [Export] public float ZoomMin       { get; set; } = 1f;
    [Export] public float ZoomMax       { get; set; } = 10f;
    [Export] public float ZoomSmoothing { get; set; } = 10f;

    private float _pitchTarget;
    private float _yawTarget;
    private float _pitchCurrent;
    private float _yawCurrent;
    private float _zoomTarget;

    public override void _Ready()
    {
        _pitchCurrent = _pitchTarget = RotationDegrees.X;
        _yawCurrent   = _yawTarget   = RotationDegrees.Y;
        _zoomTarget = SpringLength;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.Escape)
        {
            Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured
                ? Input.MouseModeEnum.Visible
                : Input.MouseModeEnum.Captured;
            return;
        }

        if (Input.MouseMode != Input.MouseModeEnum.Captured)
            return;

        if (@event is InputEventMouseButton { Pressed: true } btn)
        {
            if (btn.ButtonIndex == MouseButton.WheelUp)
                _zoomTarget = Mathf.Clamp(_zoomTarget - ZoomStep, ZoomMin, ZoomMax);
            else if (btn.ButtonIndex == MouseButton.WheelDown)
                _zoomTarget = Mathf.Clamp(_zoomTarget + ZoomStep, ZoomMin, ZoomMax);
        }

        if (@event is InputEventMouseMotion mouseMotion)
        {
            _pitchTarget = Mathf.Clamp(
                _pitchTarget - mouseMotion.Relative.Y * MouseSensitivity,
                MinPitch, MaxPitch);
            _yawTarget -= mouseMotion.Relative.X * MouseSensitivity;
        }
    }

    public override void _Process(double delta)
    {
        float t = 1f - Mathf.Exp(-SmoothSpeed * (float)delta);

        if (UseSmoothFollow)
        {
            _pitchCurrent = Mathf.Lerp(_pitchCurrent, _pitchTarget, t);
            _yawCurrent   = Mathf.Lerp(_yawCurrent,   _yawTarget,   t);
        }
        else
        {
            _pitchCurrent = _pitchTarget;
            _yawCurrent   = _yawTarget;
        }

        RotationDegrees = new Vector3(_pitchCurrent, _yawCurrent, 0f);

        float zt = 1f - Mathf.Exp(-ZoomSmoothing * (float)delta);
        SpringLength = Mathf.Lerp(SpringLength, _zoomTarget, zt);
    }
}
