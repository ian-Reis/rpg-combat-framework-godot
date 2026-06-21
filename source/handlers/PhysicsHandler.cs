using Godot;
using Helpers;

namespace Handlers;

public static class PhysicsHandler
{
    public static void ApplyGravity(Pawn pawn, float delta)
    {
        if (pawn == null) return;
        var gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
        CharacterBodyHelper.ApplyGravity(pawn, gravity, delta);
    }
}
