using Godot;
using Components;

namespace Data;

[GlobalClass]
public partial class StatusEffect : Resource
{
    public enum EffectType { Poison, Burn, Stun, Slow }

    [Export] public string     EffectId    = "";
    [Export] public EffectType Type        = EffectType.Poison;
    [Export] public float      Duration    = 3f;
    [Export] public bool       IsStackable = false;

    public virtual void OnApply(StatusEffectComponent component)             { }
    public virtual void OnTick(StatusEffectComponent component, float delta) { }
    public virtual void OnRemove(StatusEffectComponent component)            { }
}
