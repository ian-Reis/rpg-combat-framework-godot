using Godot;

// ReSharper disable once CheckNamespace
namespace Interfaces;

public interface IHasSoundEffect
{
    public AudioStreamPlayer3D SoundEffect { get; set; }
}