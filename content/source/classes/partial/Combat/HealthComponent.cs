using Godot;

namespace RPGFramework.Core;

[GlobalClass]
public partial class HealthComponent : Node
{
    [Signal] public delegate void HealthChangedEventHandler(float current, float max);
    [Signal] public delegate void DiedEventHandler();

    [Export] public float MaxHealth { get; set; } = 100f;

    [Export] public float Current { get; private set; }
    public bool  IsDead => Current <= 0f;

    public override void _Ready() => Current = MaxHealth;

    public void TakeDamage(float amount, Node3D source = null)
    {
        if (IsDead || amount <= 0f) return;

        Current = Mathf.Max(0f, Current - amount);
        GD.Print($"[Health:{GetParent()?.Name}] -{amount} de {source?.Name ?? "?"} → {Current}/{MaxHealth}");
        EmitSignal(SignalName.HealthChanged, Current, MaxHealth);

        if (IsDead)
        {
            GD.Print($"[Health:{GetParent()?.Name}] MORREU");
            EmitSignal(SignalName.Died);
        }
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f) return;

        Current = Mathf.Min(MaxHealth, Current + amount);
        GD.Print($"[Health:{GetParent()?.Name}] +{amount} (cura) → {Current}/{MaxHealth}");
        EmitSignal(SignalName.HealthChanged, Current, MaxHealth);
    }
}
