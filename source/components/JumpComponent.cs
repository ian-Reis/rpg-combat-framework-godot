using System.Diagnostics;
using Godot;
using Helpers;

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

        float dt = (float)delta;

        switch (_systemLogicComponents.Pawn)
        {
            case CharacterBody3D character:
                ProcessCharacterJump(character, dt);
                break;
            case RigidBody3D rigidBody:
                ProcessRigidBodyJump(rigidBody, dt);
                break;
        }
    }

    private void ProcessCharacterJump(CharacterBody3D character, float dt)
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

    private void ProcessRigidBodyJump(RigidBody3D rigidBody, float dt)
    {
        bool isGrounded = RigidBodyHelper.IsGrounded(rigidBody);

        if (Input.IsActionJustPressed("jump") && isGrounded)
        {
            RigidBodyHelper.ApplyJump(rigidBody, _systemLogicComponents.Stats.JumpForce);
            _isJumping = true;
            _jumpTimer = _systemLogicComponents.Stats.JumpHoldTime;
        }

        // Mantém força ascendente enquanto segura o botão (jump hold)
        if (Input.IsActionPressed("jump") && _isJumping && _jumpTimer > 0f)
        {
            rigidBody.ApplyCentralForce(Vector3.Up * _systemLogicComponents.Stats.JumpForce * 0.5f);
            _jumpTimer -= dt;
        }

        if (Input.IsActionJustReleased("jump"))
        {
            if (rigidBody.LinearVelocity.Y > 0f)
                rigidBody.LinearVelocity = new Vector3(
                    rigidBody.LinearVelocity.X,
                    rigidBody.LinearVelocity.Y * _systemLogicComponents.Stats.CutJumpFactor,
                    rigidBody.LinearVelocity.Z
                );
            _isJumping = false;
        }
    }
}
