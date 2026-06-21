using Godot;
using Components;
using Data;

namespace Helpers;

public static class CharacterBodyHelper
{
    // ── Movement ──────────────────────────────────────────────────────────────

    public static void ApplyMovement(CharacterBody3D pawn, PawnStats stats, SpringArm3D springArm, float delta, bool canRun = true, float speedMult = 1f)
    {
        if (stats == null) return;

        Vector2 inputDir = InputHelper.GetInputDirection();
        Vector3 up       = pawn.UpDirection.Normalized();

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

        float targetSpeed = (canRun && Input.IsActionPressed("run")
            ? stats.RunSpeed
            : stats.WalkSpeed) * speedMult;

        bool isGrounded = pawn.IsOnFloor();
        float accel = hasInput
            ? (isGrounded ? stats.Acceleration    : stats.AirAcceleration)
            : (isGrounded ? stats.Deceleration    : stats.AirDeceleration);

        Vector3 target       = moveDir * targetSpeed;
        Vector3 currentHoriz = GetHorizontalVelocity(pawn);
        Vector3 newHoriz     = currentHoriz.MoveToward(target, accel * delta);

        float verticalSpeed = pawn.Velocity.Dot(up);
        pawn.Velocity = newHoriz + up * verticalSpeed;
    }

    // ── Physics ───────────────────────────────────────────────────────────────

    public static void MoveAndSlide(CharacterBody3D pawn, float pushForce = 5f)
    {
        pawn.MoveAndSlide();

        for (int i = 0; i < pawn.GetSlideCollisionCount(); i++)
        {
            var collision = pawn.GetSlideCollision(i);
            if (collision.GetCollider() is RigidBody3D rb)
                rb.ApplyImpulse(-collision.GetNormal() * pushForce, collision.GetPosition() - rb.GlobalPosition);
        }
    }

    public static void ApplyGravity(CharacterBody3D pawn, float gravity, float delta)
    {
        var velocity = pawn.Velocity;
        velocity.Y -= gravity * delta;
        pawn.Velocity = velocity;
    }

    // ── Jump ──────────────────────────────────────────────────────────────────

    public static void ApplyJump(CharacterBody3D pawn, PawnStats stats)
    {
        if (!Input.IsActionJustPressed("jump") || !pawn.IsOnFloor()) return;

        Node3D planet = (Node3D)pawn.Get(Classes.statics.EntityProps.CurrentPlanet);
        if (planet == null) return;

        float jumpForce = stats?.JumpForce ?? 8f;
        Vector3 up      = (pawn.GlobalPosition - planet.GlobalPosition).Normalized();

        pawn.Velocity -= up * pawn.Velocity.Dot(up);
        pawn.Velocity += up * jumpForce;
    }

    // ── Utilities ─────────────────────────────────────────────────────────────

    public static bool IsOnFloor(CharacterBody3D pawn) => pawn.IsOnFloor();

    public static Vector3 GetHorizontalVelocity(CharacterBody3D pawn)
        => new Vector3(pawn.Velocity.X, 0f, pawn.Velocity.Z);

    public static float GetHorizontalSpeed(CharacterBody3D pawn)
        => GetHorizontalVelocity(pawn).Length();

    public static bool IsMoving(CharacterBody3D pawn, float threshold = 0.1f)
        => GetHorizontalSpeed(pawn) > threshold;
}
