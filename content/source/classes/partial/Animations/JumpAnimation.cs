using Godot;

namespace RPGFramework.Core;

public partial class DefaultControllerAnimation
{
    private bool  _wasOnFloor = true;
    private float _jumpBlend  = 0f;

    private void UpdateJump(float dt)
    {
        bool isOnFloor  = Pawn.IsOnFloor();
        float velocityY = Pawn.Velocity.Y;

        bool justJumped = _wasOnFloor && !isOnFloor && velocityY > 0f;
        bool justLanded = !_wasOnFloor && isOnFloor;

        if (justJumped)
            _jumpSMPlayback?.Start("Jump_Start");

        if (justLanded)
            _jumpSMPlayback?.Travel("Jump_Land");

        float target = isOnFloor ? 0f : 1f;
        float speed  = isOnFloor ? LandBlendSpeed : JumpBlendSpeed;
        _jumpBlend = Mathf.Lerp(_jumpBlend, target, 1f - Mathf.Exp(-speed * dt));

        AnimTree.Set(JumpBlendParam, _jumpBlend);

        _wasOnFloor = isOnFloor;
    }
}
