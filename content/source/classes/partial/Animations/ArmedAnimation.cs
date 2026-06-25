using Godot;

namespace RPGFramework.Core;

public partial class AnimationController
{
    private float _armedBlend = 0f;

    // Blend desarmado↔armado: 0 = pose normal, 1 = pose de espada.
    // Dirigido pelo estado: PawnState.IsArmed (setado pelo WeaponComponent ao equipar).
    private void UpdateArmed(float dt)
    {
        float target = Pawn?.State?.IsArmed == true ? 1f : 0f;
        _armedBlend = Mathf.Lerp(_armedBlend, target, 1f - Mathf.Exp(-ArmedBlendSpeed * dt));
        if (Parameters.BlendTo.TryGetValue("armed", out var armedBTPath))
            AnimTree.Set(armedBTPath, _armedBlend);
    }
}
