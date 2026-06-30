using Godot;

namespace RPGFramework.Entitys;

public partial class Pawn3D
{
    private void Jump(ref Vector3 velocity)
    {
        if (State?.IsDead == true) return;
        if (IsOnFloor() && ReadJump() && State.CanJump)
            velocity.Y = Stats?.JumpForce ?? 5f;
    }
}