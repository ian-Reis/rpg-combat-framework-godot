using Godot;
using Handlers;
using Helpers;
using Interfaces;
using Constants;
using Components;

namespace Resources.states;

[GlobalClass]
public partial class PlayerStateIdle : PlayerState
{
    public override void Enter(StateMachineComponent stateMachineComponent) { }

    public override void PhysicsUpdate(StateMachineComponent stateMachineComponent, float delta)
    {
        if (stateMachineComponent?.systemLogicContext is not ISystemLogicContext context) return;

        // JumpHandler.ApplyJump(context);
        // JumpHandler.JumpTravel(context);

        switch (context.Pawn)
        {
            case CharacterBody3D:
                PhysicsHandler.ApplyGravity(context, delta);
                MovementHandler.MoveAndSlide(context);
                break;
            case RigidBody3D rigidBody:
                RigidBodyHelper.BrakeHorizontal(rigidBody, 0f, 15f * delta);
                break;
        }

        if (InputHelper.GetInputDirection().Length() > 0f)
            stateMachineComponent.ChangeState(PlayerStateNames.Walk);
    }

    public override void Exit(StateMachineComponent stateMachineComponent) { }
}
