using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Components;
using Data;

[GlobalClass]
public partial class Pawn : CharacterBody3D
{
    [Export] public PawnStats Stats { get; set; }

    // Core framework components
    public LogicStateMachineComponent     LogicSM     => GetComponent<LogicStateMachineComponent>();
    public AnimationStateMachineComponent AnimationSM => GetComponent<AnimationStateMachineComponent>();
    public HealthComponent                Health      => GetComponent<HealthComponent>();
    public JumpComponent                  Jump        => GetComponent<JumpComponent>();
    public StatusEffectComponent          StatusFX    => GetComponent<StatusEffectComponent>();
    public HitboxComponent                Hitbox      => GetComponent<HitboxComponent>();
    public HurtboxComponent               Hurtbox     => GetComponent<HurtboxComponent>();
    public CameraComponent                Camera      => GetComponent<CameraComponent>();

    // AI components
    public BrainComponent       Brain     => GetComponent<BrainComponent>();
    public AIDetectionComponent Detection => GetComponent<AIDetectionComponent>();
    public AIMemoryComponent    Memory    => GetComponent<AIMemoryComponent>();
    public AIPatrolComponent    Patrol    => GetComponent<AIPatrolComponent>();
    public AIAlertComponent     Alert     => GetComponent<AIAlertComponent>();

    private readonly Dictionary<Type, Node> _componentCache = new();

    public T GetComponent<T>() where T : Node
    {
        var type = typeof(T);
        if (_componentCache.TryGetValue(type, out var cached))
            return cached as T;

        var found = GetChildren().OfType<T>().FirstOrDefault();
        if (found != null)
            _componentCache[type] = found;

        return found;
    }

    // Called by deeply nested components (HurtboxComponent, HitboxComponent) that
    // live under bones/weapons and cannot be found by GetChildren().
    public void RegisterComponent(Node component)
    {
        if (component == null) return;
        _componentCache[component.GetType()] = component;
    }
}
