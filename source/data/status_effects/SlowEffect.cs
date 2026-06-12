using Godot;

namespace Data;

[GlobalClass]
public partial class SlowEffect : StatusEffect
{
    [Export] public float SpeedMultiplier = 0.5f;

    public SlowEffect()
    {
        EffectId = "slow";
        Type     = EffectType.Slow;
        Duration = 3f;
    }
}
