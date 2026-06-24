using Godot;

namespace RPGFramework.Core;

public partial class DefaultControllerAnimation
{
    private void Init()
    {
        _movementPB?.Travel("Locomotion");
    }

    private void UpdateLocomotion(float horizontalSpeed)
    {
        float maxSpeed = Stats?.Speed ?? 5f;
        float blendValue = Mathf.Clamp(horizontalSpeed / maxSpeed, 0f, 1f);

        if (BlendSpace1DPosition.TryGetValue("locomotion", out var path))
            AnimTree.Set(path, blendValue);
    }
}
