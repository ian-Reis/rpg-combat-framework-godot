using Godot;
using Components;

namespace Data;

[GlobalClass]
public partial class PoisonEffect : StatusEffect
{
    [Godot.Export] public float DamagePerSecond = 5f;

    public PoisonEffect()
    {
        EffectId = "poison";
        Type     = EffectType.Poison;
        Duration = 5f;
    }

    public override void OnTick(StatusEffectComponent component, float delta)
        => component.Health?.TakeDamage(DamagePerSecond * delta);
}
