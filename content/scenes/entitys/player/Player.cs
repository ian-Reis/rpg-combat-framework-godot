using Godot;
using System;
using RPGFramework.Core;
using RPGFramework.Resources;
using RPGFramework.Entities;

public partial class Player : Pawn3D
{
    [Export] public HealthComponent HealthComponent { get; set; }

    public override void _Ready()
    {
        base._Ready();
        
        // Signals
        HealthComponent.HealthChanged += OnHealthChanged;
        
        // Inicializing Hud
        RootCanvasLayer.Self.Hud.SetHealthMaxMinValue(HealthComponent.MaxHealth, 0f);

    }

    private void OnHealthChanged(float current, float maxHealth)
    {
        // RootCanvasLayer.Self
    }

}
