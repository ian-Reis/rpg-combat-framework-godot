using Godot;

namespace RPGFramework.Entities;

public partial class Pawn3D
{
    private void ApplyGravity(ref Vector3 velocity, float dt)
    {
        if (!IsOnFloor())
            velocity.Y -= ProjectGravity * (Stats?.GravityScale ?? 1f) * dt;
    }
}