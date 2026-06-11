using System.Diagnostics;
using Godot;

namespace Components;

[GlobalClass]
public partial class JumpComponent : Node
{
    private float _jumpTimer = 0f;
    private bool  _isJumping = false;

    private SystemLogicComponents _systemLogicComponents;

    public override void _Ready()
    {
        _systemLogicComponents = GetParentOrNull<SystemLogicComponents>();
        Debug.Assert(_systemLogicComponents != null, "JumpComponent must be a child of SystemLogicComponents");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_systemLogicComponents?.Stats == null) return;
        if (_systemLogicComponents.Pawn is not CharacterBody3D character) return;

        float dt = (float)delta;
        ProcessJump(character, dt);
    }

    private void ProcessJump(CharacterBody3D character, float dt)
    {
        var  velocity  = character.Velocity;
        bool isOnFloor = character.IsOnFloor();

        if (Input.IsActionJustPressed("jump") && isOnFloor)
        {
            velocity.Y = _systemLogicComponents.Stats.JumpForce;
            _isJumping = true;
            _jumpTimer = _systemLogicComponents.Stats.JumpHoldTime;
            character.Velocity = velocity;
        }

        if (Input.IsActionPressed("jump") && _isJumping && _jumpTimer > 0f)
        {
            velocity.Y -= _systemLogicComponents.Stats.Gravity * dt;
            _jumpTimer -= dt;
            character.Velocity = velocity;
        }

        if (Input.IsActionJustReleased("jump") && character.Velocity.Y > 0f)
        {
            velocity.Y *= _systemLogicComponents.Stats.CutJumpFactor;
            _isJumping = false;
            character.Velocity = velocity;
        }
    }
}
