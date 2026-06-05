using Godot;
using Components;

namespace Helpers;

public static class MovementHelper
{
    public static void ApplyMovement(SystemLogicComponents owner, float delta, bool ignoreAirControl = false, bool canRun = true)
    {
        if (owner?.Pawn is not CharacterBody3D pawn) return;
        if (owner.Stats == null) return;

        Vector2 inputDir = GetInputDirection();
        Vector3 velocity = pawn.Velocity;

        Vector3 up = pawn.UpDirection.Normalized();

        SpringArm3D springArm = owner.GetComponent<CameraComponent>()?.SpringArm;
        Basis camBasis = springArm is not null
            ? springArm.GlobalTransform.Basis
            : pawn.GlobalTransform.Basis;

        Vector3 camForward = -camBasis.Z;
        Vector3 camRight   =  camBasis.X;

        Vector3 forward = (camForward - up * camForward.Dot(up)).Normalized();
        Vector3 right   = (camRight   - up * camRight.Dot(up)).Normalized();

        bool hasInput = inputDir.Length() > 0.1f;
        Vector3 moveDir = hasInput
            ? (right * inputDir.X + forward * -inputDir.Y).Normalized()
            : Vector3.Zero;

        float targetSpeed = canRun && Godot.Input.IsActionPressed("run")
            ? owner.Stats.RunSpeed
            : owner.Stats.WalkSpeed;

        float verticalSpeed = velocity.Dot(up);

        pawn.Velocity = moveDir * targetSpeed + up * verticalSpeed;
    }

    public static Vector2 GetInputDirection() => Godot.Input.GetVector("left", "right", "forward", "forback");
}
