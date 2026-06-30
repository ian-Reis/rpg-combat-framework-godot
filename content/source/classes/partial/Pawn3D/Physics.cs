using Godot;

namespace RPGFramework.Entitys;

public partial class Pawn3D
{
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