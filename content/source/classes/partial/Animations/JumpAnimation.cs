using Godot;

namespace RPGFramework.Core;

public partial class DefaultControllerAnimation
{
    private bool _wasOnFloor = true;

    private void UpdateJump()
    {
        bool isOnFloor = Pawn.IsOnFloor();
        float velocityY = Pawn.Velocity.Y;

        GD.Print($"[Jump] isOnFloor={isOnFloor} | wasOnFloor={_wasOnFloor} | VelocityY={velocityY:F2} | current='{_playback?.GetCurrentNode()}'");

        if (_playback == null) return;

        if (_wasOnFloor && !isOnFloor && velocityY > 0f)
        {
            GD.Print("[Jump] -> travel Jump_Start");
            _playback.Travel("Jump_Start");
        }
        else if (!_wasOnFloor && isOnFloor)
        {
            GD.Print("[Jump] -> start Jump_Land");
            _playback.Start("Jump_Land");
        }

        _wasOnFloor = isOnFloor;
    }
}
