using Godot;
using RPGFramework.Resources;

namespace RPGFramework.Entities;

public partial class Pawn3D
{
    private void Crouch(float dt)
    {
        // Hold: agacha só no chão e se tiver permissão. Solta = levanta.
        State.IsCrouching = IsOnFloor() && State?.CanCrouch is not false && ReadCrouch() && State.CurrentAction != PawnState.Action.Attacking;

        if (_capsule == null) return;

        // Redimensiona o colisor suavemente mantendo os pés no chão.
        float target = State.IsCrouching ? CrouchHeight : _standHeight;
        _currentHeight = Mathf.Lerp(_currentHeight, target, 1f - Mathf.Exp(-CrouchTransitionSpeed * dt));
        _capsule.Height = _currentHeight;

        Vector3 p = Collider.Position;
        p.Y = _colliderBottomY + _currentHeight * 0.5f;
        Collider.Position = p;
    }
}