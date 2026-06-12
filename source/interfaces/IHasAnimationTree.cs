using Godot;

// ReSharper disable once CheckNamespace
namespace Interfaces;

public interface IHasAnimationTree
{
    public AnimationTree AnimationTree { get; }
    public string PlaybackPath { get; }
}