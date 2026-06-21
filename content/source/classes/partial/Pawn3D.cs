using Godot;
using RPGFramework.Core;
using RPGFramework.Resources;

namespace RPGFramework.Entitys;

[GlobalClass]
public partial class Pawn3D : CharacterBody3D
{
    [ExportGroup("References Nodes")]
    [Export] public SpringArm3D SpringArm { get; set; }
    
    [ExportGroup("Resources")]
    [Export] public PawnStats Stats { get; set; }

    public Vector2 Motion { get; private set; }

    private static readonly float ProjectGravity =
        ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        Vector3 velocity = Velocity;

        ApplyGravity(ref velocity, dt);
        HandleJump(ref velocity);
        HandleMovement(ref velocity, dt);

        Velocity = velocity;
        MoveAndSlide();
        PushRigidBodies();
    }

    private void ApplyGravity(ref Vector3 velocity, float dt)
    {
        if (!IsOnFloor())
            velocity.Y -= ProjectGravity * (Stats?.GravityScale ?? 1f) * dt;
    }

    private void HandleJump(ref Vector3 velocity)
    {
        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
            velocity.Y = Stats?.JumpForce ?? 5f;
    }

    private void HandleMovement(ref Vector3 velocity, float dt)
    {
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        Motion = inputDir;

        Vector3 moveDir = Vector3.Zero;
        if (SpringArm != null && inputDir != Vector2.Zero)
        {
            float yawRad = Mathf.DegToRad(SpringArm.RotationDegrees.Y);
            Vector3 forward = new(-Mathf.Sin(yawRad), 0f, -Mathf.Cos(yawRad));
            Vector3 right   = new( Mathf.Cos(yawRad), 0f, -Mathf.Sin(yawRad));
            moveDir = (forward * -inputDir.Y + right * inputDir.X).Normalized();
        }

        float speed = Stats?.Speed ?? 5f;
        float accel = Stats?.Acceleration ?? 15f;
        float fric  = Stats?.Friction ?? 10f;

        float targetX = moveDir.X * speed;
        float targetZ = moveDir.Z * speed;

        if (moveDir != Vector3.Zero)
        {
            float t = 1f - Mathf.Exp(-accel * dt);
            velocity.X = Mathf.Lerp(velocity.X, targetX, t);
            velocity.Z = Mathf.Lerp(velocity.Z, targetZ, t);
        }
        else
        {
            float t = 1f - Mathf.Exp(-fric * dt);
            velocity.X = Mathf.Lerp(velocity.X, 0f, t);
            velocity.Z = Mathf.Lerp(velocity.Z, 0f, t);
        }
    }

    private void PushRigidBodies()
    {
        float pushForce = Stats?.PushForce ?? 5f;
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision3D col = GetSlideCollision(i);
            if (col.GetCollider() is RigidBody3D rb)
                rb.ApplyImpulse(-col.GetNormal() * pushForce, col.GetPosition() - rb.GlobalPosition);
        }
    }
}
