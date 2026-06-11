using Godot;
using Classes.statics;
using Interfaces;

namespace Helpers;

public static class JumpHelper
{
    public static void ApplyJump(ISystemLogicContext context)
    {
        if (!Godot.Input.IsActionJustPressed("jump")) return;

        float jumpForce = context.Stats?.JumpForce ?? 8f;

        switch (context?.Pawn)
        {
            case CharacterBody3D charBody when charBody.IsOnFloor():
                InstantiateCharacterJump(charBody, context, jumpForce);
                break;
            case RigidBody3D rigidBody when RigidBodyHelper.IsGrounded(rigidBody):
                RigidBodyHelper.ApplyJump(rigidBody, jumpForce);
                break;
        }
    }

    public static void JumpTravel(ISystemLogicContext context)
    {
        if (!Godot.Input.IsActionPressed("jet")) return;

        float jumpForce = context.Stats?.JumpForce ?? 8f;

        switch (context?.Pawn)
        {
            case CharacterBody3D charBody:
            {
                Node3D planet = (Node3D)charBody.Get(EntityProps.CurrentPlanet);
                if (planet == null) return;

                Vector3 up = (charBody.GlobalPosition - planet.GlobalPosition).Normalized();
                charBody.Velocity -= up * charBody.Velocity.Dot(up);
                charBody.Velocity += up * jumpForce;
                break;
            }
            case RigidBody3D rigidBody:
            {
                Node3D planet = (Node3D)rigidBody.Get(EntityProps.CurrentPlanet);
                if (planet == null) return;

                Vector3 up = (rigidBody.GlobalPosition - planet.GlobalPosition).Normalized();
                RigidBodyHelper.ApplyDirectionalImpulse(rigidBody, up, jumpForce);
                break;
            }
        }
    }

    public static void InstantiateJump(ISystemLogicContext context)
    {
        float jumpForce = context.Stats?.JumpForce ?? 8f;

        switch (context?.Pawn)
        {
            case CharacterBody3D charBody:
                InstantiateCharacterJump(charBody, context, jumpForce);
                break;
            case RigidBody3D rigidBody:
                RigidBodyHelper.ApplyJump(rigidBody, jumpForce);
                break;
            default:
                GD.PrintErr("[JumpHelper] Jump not valid. Conditions not met.");
                break;
        }
    }

    private static void InstantiateCharacterJump(CharacterBody3D pawn, ISystemLogicContext context, float jumpForce)
    {
        Node3D currentPlanet = (Node3D)pawn.Get(EntityProps.CurrentPlanet);
        if (currentPlanet == null) return;

        Vector3 upDirection = (pawn.GlobalPosition - currentPlanet.GlobalPosition).Normalized();
        pawn.Velocity -= upDirection * pawn.Velocity.Dot(upDirection);
        pawn.Velocity += upDirection * jumpForce;
    }
}
