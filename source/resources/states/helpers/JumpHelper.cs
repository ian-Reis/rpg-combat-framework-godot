using Godot;
using Components;
using Classes.statics;

namespace Helpers;

public static class JumpHelper
{
    public static void ApplyJump(SystemLogicComponents owner)
    {
        if (owner?.Pawn is not CharacterBody3D pawn) return;

        if (Godot.Input.IsActionJustPressed("jump") && pawn.IsOnFloor())
            InstantiateJump(owner);
    }

    public static void JumpTravel(SystemLogicComponents owner)
    {
        if (owner?.Pawn is not CharacterBody3D pawn) return;

        Node3D currentPlanet = (Node3D)pawn.Get(EntityProps.CurrentPlanet);
        if (currentPlanet == null) return;

        if (Godot.Input.IsActionPressed("jet"))
        {
            float jumpForce = owner.Stats?.JumpForce ?? 8f;
            Vector3 upDirection = (pawn.GlobalPosition - currentPlanet.GlobalPosition).Normalized();

            var radialVelocity = upDirection * pawn.Velocity.Dot(upDirection);
            pawn.Velocity -= radialVelocity;
            pawn.Velocity += upDirection * jumpForce;
        }
    }

    public static void InstantiateJump(SystemLogicComponents owner)
    {
        if (!IsJumpValid(owner, out CharacterBody3D pawn, out Node3D currentPlanet))
        {
            GD.PrintErr("Jump not valid. Conditions not met.");
            return;
        }

        float jumpForce = owner.Stats?.JumpForce ?? 8f;
        Vector3 upDirection = (pawn.GlobalPosition - currentPlanet.GlobalPosition).Normalized();

        var radialVelocity = upDirection * pawn.Velocity.Dot(upDirection);
        pawn.Velocity -= radialVelocity;
        pawn.Velocity += upDirection * jumpForce;
    }

    private static bool IsJumpValid(SystemLogicComponents owner, out CharacterBody3D pawn, out Node3D currentPlanet)
    {
        pawn = null;
        currentPlanet = null;

        if (owner?.Pawn is not CharacterBody3D p || !p.IsOnFloor())
            return false;

        currentPlanet = (Node3D)p.Get(EntityProps.CurrentPlanet);
        pawn = p;
        return true;
    }
}
