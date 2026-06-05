using System;
using System.Diagnostics;
using Classes.statics;
using Godot;

namespace Components;

[GlobalClass] public partial class JumpComponent : Node
{
    private float _jumpTimer = 0f;
    private bool _isJumping = false;

    private SystemLogicComponents _systemLogicComponents;

    public override void _Ready()
    {
        _systemLogicComponents = GetParentOrNull<SystemLogicComponents>();

        Debug.Assert(_systemLogicComponents != null, "JumpComponent must be a child of SystemLogicComponents");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_systemLogicComponents?.Pawn is not CharacterBody3D character)
                return;

            float dt = (float)delta;
            var velocity = character.Velocity;

            bool isOnFloor = character.IsOnFloor();

            if (Input.IsActionJustPressed("jump") && isOnFloor)
            {
                float jumpForce = (float)character.Get(EntityProps.JumpForce);
                float jumpHoldTime = (float)character.Get(EntityProps.JumpHoldTime);

                velocity.Y = jumpForce;
                _isJumping = true;
                _jumpTimer = jumpHoldTime;

                character.Velocity = velocity; // Aplica a força de pulo imediatamente
            }

            if (Input.IsActionPressed("jump") && _isJumping)
            {
                if (_jumpTimer > 0)
                {
                    float gravity = (float)character.Get(EntityProps.Gravity);

                    velocity.Y += gravity * dt; // reduz efeito da gravidade
                    _jumpTimer -= dt;

                    character.Velocity = velocity; // Aplica a redução da gravidade
                }
            }

            if (Input.IsActionJustReleased("jump") && velocity.Y > 0)
            {
                float cutJumpFactor = (float)character.Get(EntityProps.CutJumpFactor); // Fator para reduzir a altura do pul

                velocity.Y *= cutJumpFactor; // corta o pulo
                _isJumping = false;

                character.Velocity = velocity; // Aplica o corte do pulo
            }
    }

}