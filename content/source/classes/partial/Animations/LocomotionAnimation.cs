using Godot;

namespace RPGFramework.Core;

public partial class AnimationController
{
    private void Init()
    {
        _movementPB?.Travel("Locomotion");
    }

    private void UpdateLocomotion(float horizontalSpeed)
    {
        float maxSpeed = Pawn?.Stats?.Speed ?? 5f;
        float blendValue = Mathf.Clamp(horizontalSpeed / maxSpeed, 0f, 1f);

        if (Parameters.BlendSpace1DPosition.TryGetValue("locomotion", out var path))
            AnimTree.Set(path, blendValue);
    }
}
