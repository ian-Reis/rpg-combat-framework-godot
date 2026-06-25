using Godot;

namespace RPGFramework.Core;

public partial class AnimationController
{
    private float _crouchBlend = 0f;

    private void UpdateCrouch(float dt, float horizontalSpeed)
    {
        // Blend em pé↔agachado: 0 = locomotion, 1 = crouch.
        float target = Pawn.IsCrouching ? 1f : 0f;
        _crouchBlend = Mathf.Lerp(_crouchBlend, target, 1f - Mathf.Exp(-CrouchBlendSpeed * dt));
        if (Parameters.BlendTo.TryGetValue("crouch", out var crouchBTPath))
            AnimTree.Set(crouchBTPath, _crouchBlend);

        // BlendSpace crouch idle↔forward pela velocidade, normalizada pela velocidade máx. agachado.
        float maxCrouch = (Pawn?.Stats?.Speed ?? 5f) * Pawn.CrouchSpeedMultiplier;
        float move = maxCrouch > 0f ? Mathf.Clamp(horizontalSpeed / maxCrouch, 0f, 1f) : 0f;
        if (Parameters.BlendSpace1DPosition.TryGetValue("crouch", out var crouchBSPath))
            AnimTree.Set(crouchBSPath, move); 
    }
}
