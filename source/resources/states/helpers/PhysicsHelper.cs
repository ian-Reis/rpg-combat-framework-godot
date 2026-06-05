using Godot;
using Components;

// ReSharper disable once CheckNamespace
namespace Helpers;

public static class PhysicsHelper
{
    public static void ApplyGravity(SystemLogicComponents owner, float delta)
    {
        if (owner?.Pawn is not CharacterBody3D pawn) return;

        var velocity = pawn.Velocity;
        var gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
        velocity.Y -= gravity * delta;
        pawn.Velocity = velocity;
    }
}
