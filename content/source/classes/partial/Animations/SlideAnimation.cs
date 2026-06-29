using System.Collections.Generic;
using Godot;

namespace RPGFramework.Core;

public partial class AnimationController
{
    [ExportGroup("Slide")]
    [Export] public float SlideBlendSpeed { get; set; } = 10f;
    [Export] public StringName AnimNameEnd { get; set; } = "Slide_Exit";

    private void SlideCallbacks()
    {
        OnAnimFinished(AnimNameEnd, () =>
        {
            if (Parameters.OneShotRequest.TryGetValue("slide", out var slideOSPath))
                AnimTree.Set(slideOSPath, (int)AnimationNodeOneShot.OneShotRequest.FadeOut);
                
                Pawn.State.IsSliding = false;
        });
    }

    private void UpdateSlide(float dt, float horizontalSpeed)
    {
        float target = Mathf.Clamp(horizontalSpeed, 0, 1);
        if (target >= 1 && Input.IsActionPressed("slide") && !Pawn.State.IsSliding)
        {
            if (Parameters.OneShotRequest.TryGetValue("slide", out var slideOSPath))
                AnimTree.Set(slideOSPath, (int)AnimationNodeOneShot.OneShotRequest.Fire);

            Pawn.State.IsSliding = true;
        }
        else if (!Input.IsActionPressed("slide") && Pawn.State.IsSliding)
        {
            
            _slidePB?.Travel(AnimNameEnd);
        }
    }
}