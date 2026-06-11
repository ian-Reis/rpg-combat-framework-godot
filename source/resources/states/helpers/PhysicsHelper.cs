using Godot;
using Interfaces;

// ReSharper disable once CheckNamespace
namespace Helpers;

public static class PhysicsHelper
{
    public static void ApplyGravity(ISystemLogicContext context, float delta)
    {
        var gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

        switch (context?.Pawn)
        {
            case CharacterBody3D charBody:
                var velocity = charBody.Velocity;
                velocity.Y -= gravity * delta;
                charBody.Velocity = velocity;
                break;

            case RigidBody3D rigidBody when rigidBody.GravityScale == 0f:
                // Only apply manually when Godot's built-in gravity is disabled on this body.
                RigidBodyHelper.ApplyCustomGravity(rigidBody, gravity, delta);
                break;
        }
    }
}
