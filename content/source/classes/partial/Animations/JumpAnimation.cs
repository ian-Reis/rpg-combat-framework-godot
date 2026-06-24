using Godot;

namespace RPGFramework.Core;

public partial class DefaultControllerAnimation
{
    private float _jumpBlend = 0f;

    private void UpdateJump(float dt)
    {
        // Blend simples locomotion↔Jump: 1 = no ar, 0 = no chão.
        float target = Pawn.IsOnFloor() ? 0f : 1f;
        _jumpBlend = Mathf.Lerp(_jumpBlend, target, 1f - Mathf.Exp(-JumpBlendSpeed * dt));
        // AnimTree.Set(JumpBlendParam, _jumpBlend);
        if (BlendTo.TryGetValue("jump", out var jumpBTPath))
            AnimTree.Set(jumpBTPath, _jumpBlend);
    }
}
