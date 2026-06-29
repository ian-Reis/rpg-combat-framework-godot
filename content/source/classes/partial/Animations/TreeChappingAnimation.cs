using Godot;
using RPGFramework.Resources;

namespace RPGFramework.Core;

public partial class AnimationController
{
    [ExportGroup("Pistol")]
    [Export] public float TreeChappingBlendSpeed  { get; set; } = 8f;   // suavidade do blend desarmado↔armado

    private float _treeChappingBlend = 0f;

    // // Blend desarmado↔armado: 0 = pose normal, 1 = pose de espada.
    // // Dirigido pelo estado: PawnState.IsArmed (setado pelo WeaponComponent ao equipar).
    private void UpdateTreeChapping(float dt)
    {
        float target = Input.IsActionPressed("tree_chapping") ? 1f : 0f;
        _treeChappingBlend = Mathf.Lerp(_treeChappingBlend, target, 1f - Mathf.Exp(-PistolBlendSpeed * dt));
        if (Parameters.BlendTo.TryGetValue("tree_chapping", out var treeChappingBTPath))
            AnimTree.Set(treeChappingBTPath, _treeChappingBlend);
    }
}
