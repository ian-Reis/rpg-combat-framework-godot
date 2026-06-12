using Godot;
using Interfaces;

namespace Data;

[GlobalClass]
public partial class StatusEffect : Resource
{
    public enum EffectType { Poison, Burn, Stun, Slow }

    [Export] public string     EffectId    = "";
    [Export] public EffectType Type        = EffectType.Poison;
    [Export] public float      Duration    = 3f;
    [Export] public bool       IsStackable = false;

    public virtual void OnApply(ISystemLogicContext context)             { }
    public virtual void OnTick(ISystemLogicContext context, float delta) { }
    public virtual void OnRemove(ISystemLogicContext context)            { }
}
