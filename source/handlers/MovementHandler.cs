using Godot;
using Helpers;

namespace Handlers;

public static class MovementHandler
{
    public static void ApplyMovement(Pawn pawn, float delta, bool canRun = true)
    {
        if (pawn == null) return;
        float speedMult = pawn.StatusFX?.GetSpeedMultiplier() ?? 1f;
        SpringArm3D springArm = pawn.Camera?.SpringArm;
        CharacterBodyHelper.ApplyMovement(pawn, pawn.Stats, springArm, delta, canRun, speedMult);
    }

    public static void MoveAndSlide(Pawn pawn, float pushForce = 5f)
    {
        if (pawn == null) return;
        CharacterBodyHelper.MoveAndSlide(pawn, pushForce);
    }
}
