using Godot;

namespace RPGFramework.Entities;

public partial class Pawn3D
{
    private void Movement(ref Vector3 velocity, float dt)
    {
        // Sem permissão de mover (State.CanMove) → moveDir zero; a fricção abaixo para suave (não desliza).
        // IsDead não é checado aqui: o _PhysicsProcess já retorna cedo quando morto.
        Vector3 moveDir = State?.CanMove is false && State?.IsSliding is false ? Vector3.Zero : ReadMoveDirection();

        float speed = Stats?.Speed ?? 5f;
        if (State.IsCrouching) speed *= CrouchSpeedMultiplier;
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
}