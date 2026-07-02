using Godot;
using RPGFramework.Resources;

namespace RPGFramework.Entities;

public partial class Pawn3D
{
    private void Slide(ref Vector3 velocity, float dt)
    {
        State.IsSliding = State?.CanSlide is true && IsOnFloor() && (GetHorizontalSpeed().Length() > 0.8f) && ReadSlide() && State.CurrentAction != PawnState.Action.Attacking;
        if (State.IsSliding)
        {
            if (_slideVelocity == Vector3.Zero)
                _slideVelocity = velocity;
            
            Vector3 target = State.IsSliding ? _slideVelocity + ReadMoveDirection() : Vector3.Zero;
            
            velocity = target;
        }
        else
        {
            _slideVelocity = Vector3.Zero;
        }
    }

}