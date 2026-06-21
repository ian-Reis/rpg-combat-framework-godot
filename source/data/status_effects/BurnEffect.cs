using Godot;
using Components;

namespace Data;

[GlobalClass]
public partial class BurnEffect : StatusEffect
{
    [Godot.Export] public float DamagePerSecond = 12f;

    public BurnEffect()
    {
        EffectId = "burn";
        Type     = EffectType.Burn;
        Duration = 3f;
    }

    public override void OnTick(StatusEffectComponent component, float delta)
        => component.Health?.TakeDamage(DamagePerSecond * delta);
}
