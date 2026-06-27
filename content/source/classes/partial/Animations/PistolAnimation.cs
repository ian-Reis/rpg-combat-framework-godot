using Godot;
using RPGFramework.Resources;

namespace RPGFramework.Core;

public partial class AnimationController
{
    private float _pistolBlend = 0f;

    // // Blend desarmado↔armado: 0 = pose normal, 1 = pose de espada.
    // // Dirigido pelo estado: PawnState.IsArmed (setado pelo WeaponComponent ao equipar).
    private void UpdatePistol(float dt)
    {
        float target = Pawn.State.IsAimed ? 1f : 0f;
        _pistolBlend = Mathf.Lerp(_pistolBlend, target, 1f - Mathf.Exp(-PistolBlendSpeed * dt));
        if (Parameters.BlendTo.TryGetValue("pistol", out var pistolBTPath))
            AnimTree.Set(pistolBTPath, _pistolBlend);
    }
}
