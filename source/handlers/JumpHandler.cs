using Godot;
using Classes.statics;
using Interfaces;

namespace Handlers;

public static class JumpHandler
{
    public static void ApplyJump(ISystemLogicContext context)
    {
        if (!Input.IsActionJustPressed("jump")) return;
        if (context?.Pawn is not CharacterBody3D charBody) return;
        if (!charBody.IsOnFloor()) return;

        InstantiateJump(charBody, context);
    }

    public static void JumpTravel(ISystemLogicContext context)
    {
        if (!Input.IsActionPressed("jet")) return;
        if (context?.Pawn is not CharacterBody3D charBody) return;

        Node3D planet = (Node3D)charBody.Get(EntityProps.CurrentPlanet);
        if (planet == null) return;

        float jumpForce = context.Stats?.JumpForce ?? 8f;
        Vector3 up = (charBody.GlobalPosition - planet.GlobalPosition).Normalized();
        charBody.Velocity -= up * charBody.Velocity.Dot(up);
        charBody.Velocity += up * jumpForce;
    }

    public static void InstantiateJump(ISystemLogicContext context)
    {
        if (context?.Pawn is not CharacterBody3D charBody) return;
        InstantiateJump(charBody, context);
    }

    private static void InstantiateJump(CharacterBody3D pawn, ISystemLogicContext context)
    {
        Node3D planet = (Node3D)pawn.Get(EntityProps.CurrentPlanet);
        if (planet == null) return;

        float jumpForce = context.Stats?.JumpForce ?? 8f;
        Vector3 up = (pawn.GlobalPosition - planet.GlobalPosition).Normalized();
        pawn.Velocity -= up * pawn.Velocity.Dot(up);
        pawn.Velocity += up * jumpForce;
    }
}
