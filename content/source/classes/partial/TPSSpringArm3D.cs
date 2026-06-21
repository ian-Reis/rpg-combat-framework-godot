using Godot;

namespace RPGFramework.Core;

[GlobalClass]
public partial class TPSSpringArm3D : SpringArm3D
{
    [ExportGroup("Rotation")]
    [Export] public float MouseSensitivity { get; set; } = 0.3f;
    [Export] public float MinPitch { get; set; } = -89f;
    [Export] public float MaxPitch { get; set; } = 89f;

    [ExportGroup("Smooth Follow")]
    [Export] public bool UseSmoothFollow { get; set; } = true;
    [Export] public float SmoothSpeed { get; set; } = 10.0f;

    private float _pitchTarget;
    private float _yawTarget;
    private float _pitchCurrent;
    private float _yawCurrent;

    public override void _Ready()
    {
        _pitchCurrent = _pitchTarget = RotationDegrees.X;
        _yawCurrent = _yawTarget = RotationDegrees.Y;
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
        if (UseSmoothFollow)
        {
            float t = 1f - Mathf.Exp(-SmoothSpeed * (float)delta);
            _pitchCurrent = Mathf.Lerp(_pitchCurrent, _pitchTarget, t);
            _yawCurrent = Mathf.Lerp(_yawCurrent, _yawTarget, t);
        }
        else
        {
            _pitchCurrent = _pitchTarget;
            _yawCurrent = _yawTarget;
        }

        RotationDegrees = new Vector3(_pitchCurrent, _yawCurrent, 0f);
    }
}
