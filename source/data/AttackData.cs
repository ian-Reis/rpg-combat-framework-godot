using Godot;

namespace Data;

[GlobalClass]
public partial class AttackData : Resource
{
    public enum AttackType { Physical, Magic, Fire, Ice, Lightning, True }

    [ExportGroup("Damage")]
    [Export] public float      Damage = 10f;
    [Export] public AttackType Type   = AttackType.Physical;

    [ExportGroup("Knockback")]
    [Export] public float KnockbackForce    = 5f;
    [Export] public float KnockbackDuration = 0.25f;

    [ExportGroup("Status Effects")]
    [Export] public StatusEffect[] Effects       = [];
    [Export] public float[]        EffectChances = []; // 0..1, parallel to Effects

    [ExportGroup("Hit Feel")]
    [Export] public bool  CanParry        = true;
    [Export] public float HitStopDuration = 0.05f;
}
