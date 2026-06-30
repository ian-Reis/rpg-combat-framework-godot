using Godot;
using RPGFramework.Resources;

namespace RPGFramework.Entities;

public partial class Pawn3D
{
    private void Slide(ref Vector3 velocity, float dt)
    {
        State.IsSliding = State?.CanSlide is true && IsOnFloor() && (GetHorizontalSpeed().Length() > 0.8f) && ReadSlide() && State.CurrentAction != PawnState.Action.Attacking;
    }

}