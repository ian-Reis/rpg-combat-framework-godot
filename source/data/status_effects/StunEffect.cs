using Godot;

namespace Data;

[GlobalClass]
public partial class StunEffect : StatusEffect
{
    public StunEffect()
    {
        EffectId = "stun";
        Type     = EffectType.Stun;
        Duration = 1f;
    }
}
