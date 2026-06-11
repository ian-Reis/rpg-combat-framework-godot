using Godot;
using Helpers;
using Interfaces;

namespace Handlers;

public static class PhysicsHandler
{
    public static void ApplyGravity(ISystemLogicContext context, float delta)
    {
        if (context?.Pawn is not CharacterBody3D charBody) return;
        var gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
        CharacterBodyHelper.ApplyGravity(charBody, gravity, delta);
    }
}
