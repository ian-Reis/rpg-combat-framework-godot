using System.Diagnostics;
using Godot;
using Classes.statics;

namespace Components;

[GlobalClass]
public partial class JumpComponent : Node
{
    private float _jumpTimer = 0f;
    private bool  _isJumping = false;

    private SystemLogicComponents _owner;

    public override void _Ready()
    {
        _owner = GetParentOrNull<SystemLogicComponents>();
        Debug.Assert(_owner != null, "JumpComponent must be a child of SystemLogicComponents");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_owner?.Stats == null) return;
        if (_owner.Pawn is not CharacterBody3D character) return;

        float dt = (float)delta;
        ProcessJump(character, dt);
        ProcessJumpTravel(character, dt);
    }

    private void ProcessJump(CharacterBody3D character, float dt)
    {
        Vector3 up        = GetUpDirection(character);
        float   vertSpeed = character.Velocity.Dot(up);
        bool    isOnFloor = character.IsOnFloor();

        if (Input.IsActionJustPressed("jump") && isOnFloor)
        {
            character.Velocity -= up * vertSpeed;
            character.Velocity += up * _owner.Stats.JumpForce;
            _isJumping = true;
            _jumpTimer = _owner.Stats.JumpHoldTime;
        }

        if (Input.IsActionPressed("jump") && _isJumping && _jumpTimer > 0f)
        {
            character.Velocity += up * _owner.Stats.Gravity * dt;
            _jumpTimer -= dt;
        }

        if (Input.IsActionJustReleased("jump") && character.Velocity.Dot(up) > 0f)
        {
            float vert = character.Velocity.Dot(up);
            character.Velocity -= up * vert;
            character.Velocity += up * vert * _owner.Stats.CutJumpFactor;
            _isJumping = false;
        }
    }

    private void ProcessJumpTravel(CharacterBody3D character, float dt)
    {
        if (!Input.IsActionPressed("jet")) return;

        Vector3 up = GetUpDirection(character);
        character.Velocity -= up * character.Velocity.Dot(up);
        character.Velocity += up * _owner.Stats.JumpForce;
    }

    private static Vector3 GetUpDirection(CharacterBody3D character)
    {
        var planet = character.Get(EntityProps.CurrentPlanet).As<Node3D>();
        if (planet != null)
            return (character.GlobalPosition - planet.GlobalPosition).Normalized();

        return character.UpDirection.Normalized();
    }
}
