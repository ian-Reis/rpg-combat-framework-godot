using System.Diagnostics;
using Godot;

namespace Components;

[GlobalClass] public partial class CameraComponent : Node
{
    [Export] public Camera3D Camera { get; set; }
    [Export] public SpringArm3D SpringArm { get; set; }
    [Export] public Node3D Model { get; set; }

    [Export] public float MouseSensitivity = 0.1f;
    [Export] public float MaxVerticalAngle = 80f;
    [Export] public float MinVerticalAngle = -80f;
    [Export] public float CameraDistance = 3.5f;
    [Export] public float SmoothVelocity = 1.5f;

    private float _pitch = 0f; // Rotação vertical
    private float _yaw = 0f;   // Rotação horizontal

    private SystemLogicComponents _systemLogicComponents;

    public override void _Ready()
    {
        _systemLogicComponents = GetParentOrNull<SystemLogicComponents>();

        Debug.Assert(_systemLogicComponents != null, "CameraConfigComponent must be a child of SystemLogicComponents");

        if (SpringArm != null)
        {
            SpringArm.SpringLength = CameraDistance;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion) RotateCamera(mouseMotion);
        if (@event is InputEventMouseButton mouseButton) ZoomCamera(mouseButton);
        PointerLock(@event);
    }

    public override void _Process(double delta)
    {
        if (Camera is null || SpringArm is null) return;
        if (_systemLogicComponents?.Pawn is not CharacterBody3D pawn) return;
        
        // 1. Pegamos o "Cima" atual do player (que o seu script de gravidade já atualiza)
        Vector3 playerUp = pawn.GlobalTransform.Basis.Y;

        // 2. Criamos a rotação do mouse usando Quaternions (evita Gimbal Lock em esferas)
        // Giramos em torno do eixo Y local do player (Yaw) e do eixo X lateral (Pitch)
        Quaternion yawQuat = Quaternion.FromEuler(new Vector3(0, Mathf.DegToRad(_yaw), 0));
        Quaternion pitchQuat = Quaternion.FromEuler(new Vector3(Mathf.DegToRad(_pitch), 0, 0));
        
        // 3. Combinamos: Primeiro o Pitch, depois o Yaw
        Quaternion combinedRotation = yawQuat * pitchQuat;

        // 4. Aplicamos a base do player como referência para que a câmera "siga" a curvatura do planeta
        // Isso faz com que o (0,0,0) da câmera seja sempre alinhado com o pé do player no chão
        SpringArm.GlobalBasis = pawn.GlobalTransform.Basis * new Basis(combinedRotation);

    }

    private void RotateCamera(InputEventMouseMotion mouseMotion)
    {
        if (Camera is null || SpringArm is null) return;
        if (Input.MouseMode != Input.MouseModeEnum.Captured) return;
        
        // Removido o MouseSensitivity daqui para aplicar no cálculo final ou manter valores consistentes
        _yaw -= mouseMotion.Relative.X * MouseSensitivity;
        _pitch -= mouseMotion.Relative.Y * MouseSensitivity;

        _pitch = Mathf.Clamp(_pitch, MinVerticalAngle, MaxVerticalAngle);
    }

    private void ZoomCamera(InputEventMouseButton mouseButton)
    {
        if (Camera is null || SpringArm is null) return;
        if (Input.MouseMode != Input.MouseModeEnum.Captured) return;
        

        if (mouseButton.ButtonIndex == MouseButton.WheelUp)
        {
            CameraDistance = Mathf.Max(1f, CameraDistance - 0.5f);
        }
        else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
        {
            CameraDistance = Mathf.Min(10f, CameraDistance + 0.5f);
        }

        SpringArm.SpringLength = CameraDistance;
    }

    private void PointerLock(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured
                ? Input.MouseModeEnum.Visible
                : Input.MouseModeEnum.Captured;
        }
    }

}