using Godot;
using Interfaces;

// ReSharper disable once CheckNamespace
namespace Helpers;

public static class PhysicsHelper
{
    public static void ApplyGravity(ISystemLogicContext context, float delta)
    {
        if (context?.Pawn is not CharacterBody3D pawn) return;

        var velocity = pawn.Velocity;
        var gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
        velocity.Y -= gravity * delta;
        pawn.Velocity = velocity;
    }
}
