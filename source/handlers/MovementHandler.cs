using Helpers;
using Interfaces;

namespace Handlers;

public static class MovementHandler
{
    public static void ApplyMovement(ISystemLogicContext context, float delta, bool ignoreAirControl = false, bool canRun = true)
    {
        if (context?.Pawn is not Godot.CharacterBody3D charBody) return;
        CharacterBodyHelper.ApplyMovement(charBody, context, canRun);
    }

    public static void MoveAndSlide(ISystemLogicContext context, float pushForce = 5f)
    {
        if (context?.Pawn is not Godot.CharacterBody3D pawn) return;
        CharacterBodyHelper.MoveAndSlide(pawn, pushForce);
    }
}
