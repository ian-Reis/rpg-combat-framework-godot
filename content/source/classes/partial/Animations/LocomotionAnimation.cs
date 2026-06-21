using Godot;

namespace RPGFramework.Core;

public partial class DefaultControllerAnimation
{
    private void UpdateLocomotion(float horizontalSpeed)
    {
        float maxSpeed = Stats?.Speed ?? 5f;
        float blendValue = Mathf.Clamp(horizontalSpeed / maxSpeed, 0f, 1f);
        AnimTree.Set(LocomotionBlendParam, blendValue);
    }
}
