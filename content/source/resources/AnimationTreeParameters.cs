using Godot;

namespace RPGFramework.Resources;

[GlobalClass]
public partial class AnimationTreeParameters : Resource
{
    [Export] public Godot.Collections.Dictionary<StringName, string> BlendTo              = new();
    [Export] public Godot.Collections.Dictionary<StringName, string> BlendSpace1DPosition = new();
    [Export] public Godot.Collections.Dictionary<StringName, string> Playback             = new();
    [Export] public Godot.Collections.Dictionary<StringName, string> TransitionRequest    = new();
    [Export] public Godot.Collections.Dictionary<StringName, string> OneShotRequest       = new();
}