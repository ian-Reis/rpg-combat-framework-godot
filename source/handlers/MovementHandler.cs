using Godot;
using Helpers;
using Interfaces;

namespace Handlers;

public static class MovementHandler
{
    public static void ApplyMovement(ISystemLogicContext context, float delta, bool ignoreAirControl = false, bool canRun = true)
    {
        switch (context?.Pawn)
        {
            case CharacterBody3D charBody:
                CharacterBodyHelper.ApplyMovement(charBody, context, canRun);
                break;
            case RigidBody3D rigidBody:
                RigidBodyHelper.ApplyMovementVelocity(rigidBody, context, canRun);
                break;
        }
    }

    /// <summary>
    /// Calls MoveAndSlide for CharacterBody3D. No-op for RigidBody3D (physics engine handles it).
    /// </summary>
    public static void MoveAndSlide(ISystemLogicContext context)
    {
        if (context?.Pawn is CharacterBody3D pawn)
            CharacterBodyHelper.MoveAndSlide(pawn);
    }

    public static Vector2 GetInputDirection() => Input.GetVector("left", "right", "forward", "forback");
}
