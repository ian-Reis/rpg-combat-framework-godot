using Godot;
using Components;
using Interfaces;

namespace Data;

[GlobalClass]
public partial class BurnEffect : StatusEffect
{
    [Export] public float DamagePerSecond = 12f;

    public BurnEffect()
    {
        EffectId = "burn";
        Type     = EffectType.Burn;
        Duration = 3f;
    }

    public override void OnTick(ISystemLogicContext context, float delta)
        => context.GetComponent<HealthComponent>()?.TakeDamage(DamagePerSecond * delta);
}
