using Godot;
using Components;
using Interfaces;

namespace Data;

[GlobalClass]
public partial class PoisonEffect : StatusEffect
{
    [Export] public float DamagePerSecond = 5f;

    public PoisonEffect()
    {
        EffectId = "poison";
        Type     = EffectType.Poison;
        Duration = 5f;
    }

    public override void OnTick(ISystemLogicContext context, float delta)
        => context.GetComponent<HealthComponent>()?.TakeDamage(DamagePerSecond * delta);
}
