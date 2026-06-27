using Godot;

namespace RPGFramework.Core;

public partial class AnimationController
{
    [ExportGroup("Slide")]
    [Export] public float SlideBlendSpeed { get; set; } = 10f;
}