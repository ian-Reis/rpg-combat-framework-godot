using Godot;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Data;

// ReSharper disable once CheckNamespace
namespace Components;

[GlobalClass]
public partial class SystemLogicComponents : Node, ISystemLogicContext
{
    [ExportGroup("References")]
    [Export] public Node3D Pawn { get; set; }

    [ExportGroup("Stats")]
    [Export] public CharacterStats Stats { get; set; }

    public AudioStreamPlayer3D SoundEffect => GetComponent<AudioStreamPlayer3D>();
    public StateMachineComponent StateMachineComponent => GetComponent<StateMachineComponent>();

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
