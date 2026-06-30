using System.Collections.Generic;
using Godot;

namespace RPGFramework.Core;

public partial class AnimationController
{
    [ExportGroup("Slide")]
    [Export] public float SlideBlendSpeed { get; set; } = 10f;  // suavidade do blend em pé↔agachado
    private float _slideBlend = 0f;

    private void UpdateSlide(float dt, float horizontalSpeed)
    {
        // Blend em pé↔agachado: 0 = locomotion, 1 = crouch.
        float target = Pawn.State.IsSliding ? 1f : 0f;
        _slideBlend = Mathf.Lerp(_slideBlend, target, 1f - Mathf.Exp(-SlideBlendSpeed * dt));
        if (Parameters.BlendTo.TryGetValue("slide", out var slideBTPath))
            AnimTree.Set(slideBTPath, _slideBlend);
    }
}