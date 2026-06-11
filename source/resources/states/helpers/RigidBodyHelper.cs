using Godot;
using Components;
using Interfaces;

namespace Helpers;

public static class RigidBodyHelper
{
    // ── Direction ─────────────────────────────────────────────────────────────

    public static Vector3 GetWorldMoveDirection(RigidBody3D body, ISystemLogicContext context)
    {
        Vector2 inputDir = InputHelper.GetInputDirection();
        if (inputDir.Length() < 0.1f) return Vector3.Zero;

        SpringArm3D springArm = context.GetComponent<CameraComponent>()?.SpringArm;
        Basis camBasis = springArm is not null
            ? springArm.GlobalTransform.Basis
            : body.GlobalTransform.Basis;

        Vector3 up       = Vector3.Up;
        Vector3 camFwd   = -camBasis.Z;
        Vector3 camRight =  camBasis.X;

        Vector3 forward = (camFwd   - up * camFwd.Dot(up)).Normalized();
        Vector3 right   = (camRight - up * camRight.Dot(up)).Normalized();

        return (right * inputDir.X + forward * -inputDir.Y).Normalized();
    }

    // ── Velocity-based (direct) ───────────────────────────────────────────────

    public static void SetHorizontalVelocity(RigidBody3D body, Vector3 direction, float speed)
    {
        body.LinearVelocity = direction * speed + Vector3.Up * body.LinearVelocity.Y;
    }

    /// <summary>
    /// Reads input and instantly drives the body toward the target speed.
    /// Stops immediately when no input. Good for tight, responsive controls.
    /// </summary>
    public static void ApplyMovementVelocity(RigidBody3D body, ISystemLogicContext context, bool canRun = true)
    {
        if (context?.Stats == null) return;

        Vector3 moveDir = GetWorldMoveDirection(body, context);
        float targetSpeed = canRun && Input.IsActionPressed("run")
            ? context.Stats.RunSpeed
            : context.Stats.WalkSpeed;

        if (moveDir.Length() > 0.1f)
            SetHorizontalVelocity(body, moveDir, targetSpeed);
        else
            BrakeHorizontal(body, 0f);
    }

    // ── Force-based ───────────────────────────────────────────────────────────

    /// <summary>
    /// Accelerates the body toward target speed using forces.
    /// More physically realistic — preserves momentum, feels weighty.
    /// </summary>
    public static void ApplyMovementForce(RigidBody3D body, ISystemLogicContext context, float acceleration, bool canRun = true)
    {
        if (context?.Stats == null) return;

        Vector3 moveDir = GetWorldMoveDirection(body, context);
        float targetSpeed = canRun && Input.IsActionPressed("run")
            ? context.Stats.RunSpeed
            : context.Stats.WalkSpeed;

        Vector3 currentH = GetHorizontalVelocity(body);
        Vector3 targetH  = moveDir * targetSpeed;
        Vector3 diff     = targetH - currentH;

        if (diff.LengthSquared() > 0.01f)
            body.ApplyCentralForce(diff.Normalized() * acceleration);
    }

    // ── Jump ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Instantly applies a vertical impulse. Resets vertical velocity first for consistent height.
    /// </summary>
    public static void ApplyJump(RigidBody3D body, float jumpForce)
    {
        body.LinearVelocity = new Vector3(body.LinearVelocity.X, 0f, body.LinearVelocity.Z);
        body.ApplyCentralImpulse(Vector3.Up * jumpForce);
    }

    public static void ApplyJumpIfInput(RigidBody3D body, float jumpForce, bool isGrounded)
    {
        if (Input.IsActionJustPressed("jump") && isGrounded)
            ApplyJump(body, jumpForce);
    }

    // ── Speed cap ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Clamps horizontal speed to maxSpeed without affecting vertical velocity.
    /// Call every physics frame when using force-based movement.
    /// </summary>
    public static void CapHorizontalSpeed(RigidBody3D body, float maxSpeed)
    {
        Vector3 horizontal = GetHorizontalVelocity(body);
        if (horizontal.Length() <= maxSpeed) return;

        Vector3 clamped = horizontal.Normalized() * maxSpeed;
        body.LinearVelocity = new Vector3(clamped.X, body.LinearVelocity.Y, clamped.Z);
    }

    // ── Braking ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Lerps horizontal speed toward targetSpeed. Use lerpFactor 1f for instant stop.
    /// </summary>
    public static void BrakeHorizontal(RigidBody3D body, float targetSpeed, float lerpFactor = 1f)
    {
        Vector3 horizontal = GetHorizontalVelocity(body);
        if (horizontal.Length() <= targetSpeed + 0.01f) return;

        Vector3 braked = horizontal.Lerp(horizontal.Normalized() * targetSpeed, Mathf.Clamp(lerpFactor, 0f, 1f));
        body.LinearVelocity = new Vector3(braked.X, body.LinearVelocity.Y, braked.Z);
    }

    /// <summary>
    /// Applies an opposing force to decelerate. Use with force-based movement.
    /// </summary>
    public static void ApplyBrakeForce(RigidBody3D body, float brakeForce)
    {
        Vector3 horizontal = GetHorizontalVelocity(body);
        if (horizontal.LengthSquared() < 0.01f) return;

        body.ApplyCentralForce(-horizontal.Normalized() * brakeForce);
    }

    // ── Gravity ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Manually adds downward gravity. Only use when body.GravityScale is 0.
    /// </summary>
    public static void ApplyCustomGravity(RigidBody3D body, float gravity, float delta)
    {
        body.LinearVelocity += Vector3.Down * gravity * delta;
    }

    /// <summary>
    /// Gravity toward an arbitrary direction — for planet gravity setups.
    /// </summary>
    public static void ApplyDirectionalGravity(RigidBody3D body, Vector3 gravityDir, float gravity, float delta)
    {
        body.LinearVelocity += gravityDir.Normalized() * gravity * delta;
    }

    // ── Ground detection ──────────────────────────────────────────────────────

    public static bool IsGrounded(RigidBody3D body, float checkDistance = 0.15f)
    {
        var spaceState = body.GetWorld3D().DirectSpaceState;
        var from = body.GlobalPosition;
        var to   = from + Vector3.Down * checkDistance;

        var query = PhysicsRayQueryParameters3D.Create(
            from, to,
            exclude: new Godot.Collections.Array<Rid> { body.GetRid() }
        );

        return spaceState.IntersectRay(query).Count > 0;
    }

    public static bool IsGroundedInDirection(RigidBody3D body, Vector3 downDir, float checkDistance = 0.15f)
    {
        var spaceState = body.GetWorld3D().DirectSpaceState;
        var from = body.GlobalPosition;
        var to   = from + downDir.Normalized() * checkDistance;

        var query = PhysicsRayQueryParameters3D.Create(
            from, to,
            exclude: new Godot.Collections.Array<Rid> { body.GetRid() }
        );

        return spaceState.IntersectRay(query).Count > 0;
    }

    // ── Alignment ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Smoothly rotates the body so its Y axis aligns with upDirection.
    /// Useful for planet gravity or tilted surfaces.
    /// </summary>
    public static void AlignToGravity(RigidBody3D body, Vector3 upDirection, float alignSpeed, float delta)
    {
        Vector3 currentUp = body.GlobalTransform.Basis.Y.Normalized();
        Vector3 targetUp  = upDirection.Normalized();
        Vector3 axis      = currentUp.Cross(targetUp);

        if (axis.LengthSquared() < 1e-6f) return;

        float angle    = Mathf.Min(currentUp.AngleTo(targetUp), alignSpeed * delta);
        Basis newBasis = new Basis(new Quaternion(axis.Normalized(), angle)) * body.GlobalTransform.Basis;
        body.GlobalTransform = new Transform3D(newBasis.Orthonormalized(), body.GlobalPosition);
    }

    // ── Impulses ──────────────────────────────────────────────────────────────

    public static void ApplyDirectionalImpulse(RigidBody3D body, Vector3 direction, float magnitude)
        => body.ApplyCentralImpulse(direction.Normalized() * magnitude);

    public static void ApplyKnockback(RigidBody3D body, Vector3 source, float force)
    {
        Vector3 dir = (body.GlobalPosition - source).Normalized();
        body.ApplyCentralImpulse(dir * force);
    }

    // ── Utilities ─────────────────────────────────────────────────────────────

    public static Vector3 GetHorizontalVelocity(RigidBody3D body)
        => new Vector3(body.LinearVelocity.X, 0f, body.LinearVelocity.Z);

    public static float GetHorizontalSpeed(RigidBody3D body)
        => GetHorizontalVelocity(body).Length();

    public static bool IsMoving(RigidBody3D body, float threshold = 0.1f)
        => GetHorizontalSpeed(body) > threshold;
}
