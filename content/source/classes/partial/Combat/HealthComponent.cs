using Godot;

namespace RPGFramework.Core;

[GlobalClass]
public partial class HealthComponent : Node
{
    [Signal] public delegate void HealthChangedEventHandler(float current, float max);
    [Signal] public delegate void DiedEventHandler();

    [Export] public float MaxHealth { get; set; } = 100f;
    // Opcional: se atribuído, auto-conecta no sinal Hurt e aplica o dano sozinho.
    [Export] public HurtBoxArea3D HurtBox { get; set; }

    public float Current { get; private set; }
    public bool  IsDead => Current <= 0f;

    public override void _Ready()
    {
        Current = MaxHealth;
        if (HurtBox != null)
            HurtBox.Hurt += OnHurt;
    }

    private void OnHurt(HitBoxArea3D hitBox)
    {
        TakeDamage(hitBox.Damage, hitBox.Source);
    }

    public void TakeDamage(float amount, Node3D source = null)
    {
        if (IsDead || amount <= 0f) return;

        Current = Mathf.Max(0f, Current - amount);
        EmitSignal(SignalName.HealthChanged, Current, MaxHealth);

        if (IsDead)
            EmitSignal(SignalName.Died);
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f) return;

        Current = Mathf.Min(MaxHealth, Current + amount);
        EmitSignal(SignalName.HealthChanged, Current, MaxHealth);
    }
}
