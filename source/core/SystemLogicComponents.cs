using Godot;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Data;

// ReSharper disable once CheckNamespace
namespace Components;

[GlobalClass]
public partial class SystemLogicComponents : Node,
    IComponentOwner,
    IHasAnimationTree,
    IHasSoundEffect,
    IHasStateMachineComponent
{
    [ExportGroup("References")]
    [Export] public Node3D Pawn { get; set; }
    [Export] public AnimationTree AnimationTree { get; set; }
    [Export] public AudioStreamPlayer3D SoundEffect { get; set; }
    [Export] public StateMachineComponent StateMachineComponent { get; set; }

    [ExportGroup("Stats")]
    [Export] public CharacterStats Stats { get; set; }

    public float InputConsistencyFrames;
    public Vector2 LastInputDir;
    public Vector2 FacingDirection { get; set; } = Vector2.Down;
    public bool CanJump { get; set; } = true;

    private readonly Dictionary<Type, Node> _componentCache = new();

    public T GetComponent<T>() where T : class
    {
        var type = typeof(T);
        if (_componentCache.TryGetValue(type, out var cached))
            return cached as T;

        var found = GetChildren().OfType<T>().FirstOrDefault();
        if (found is Node node)
            _componentCache[type] = node;

        return found;
    }
}
