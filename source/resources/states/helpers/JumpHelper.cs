using Godot;
using Classes.statics;
using Interfaces;

namespace Helpers;

public static class JumpHelper
{
    public static void ApplyJump(IPlayerStateContext context)
    {
        if (context?.Pawn is not CharacterBody3D pawn) return;

        if (Godot.Input.IsActionJustPressed("jump") && pawn.IsOnFloor())
            InstantiateJump(context);
    }

    public static void JumpTravel(IPlayerStateContext context)
    {
        if (context?.Pawn is not CharacterBody3D pawn) return;

        Node3D currentPlanet = (Node3D)pawn.Get(EntityProps.CurrentPlanet);
        if (currentPlanet == null) return;

        if (Godot.Input.IsActionPressed("jet"))
        {
            float jumpForce = context.Stats?.JumpForce ?? 8f;
            Vector3 upDirection = (pawn.GlobalPosition - currentPlanet.GlobalPosition).Normalized();

            var radialVelocity = upDirection * pawn.Velocity.Dot(upDirection);
            pawn.Velocity -= radialVelocity;
            pawn.Velocity += upDirection * jumpForce;
        }
    }

    public static void InstantiateJump(IPlayerStateContext context)
    {
        if (!IsJumpValid(context, out CharacterBody3D pawn, out Node3D currentPlanet))
        {
            GD.PrintErr("Jump not valid. Conditions not met.");
            return;
        }

        float jumpForce = context.Stats?.JumpForce ?? 8f;
        Vector3 upDirection = (pawn.GlobalPosition - currentPlanet.GlobalPosition).Normalized();

        var radialVelocity = upDirection * pawn.Velocity.Dot(upDirection);
        pawn.Velocity -= radialVelocity;
        pawn.Velocity += upDirection * jumpForce;
    }

    private static bool IsJumpValid(IPlayerStateContext context, out CharacterBody3D pawn, out Node3D currentPlanet)
    {
        pawn = null;
        currentPlanet = null;

        if (context?.Pawn is not CharacterBody3D p || !p.IsOnFloor())
            return false;

        currentPlanet = (Node3D)p.Get(EntityProps.CurrentPlanet);
        pawn = p;
        return true;
    }
}
