using Godot;

namespace RPGFramework.Core;

[GlobalClass]
public partial class HurtBoxArea3D : Area3D
{
    // Emitido ao ser atingida. A entidade dona escuta e trata (vida, knockback, etc).
    [Signal] public delegate void HurtEventHandler(HitBoxArea3D hitBox);

    // Layer 2 = hurtbox (ver CLAUDE.md). Ajustável no inspector.
    private const uint HurtboxLayer = 1u << 1;

    // Entidade dona desta hurtbox (ex: o Pawn). Usado pela hitbox p/ evitar auto-dano.
    [Export] public Node3D Entity { get; set; }

    public override void _Ready()
    {
        CollisionLayer = HurtboxLayer;
        CollisionMask  = 0;     // não detecta nada; é a hitbox que a detecta
        Monitoring     = false; // não precisa varrer
        Monitorable    = true;  // mas precisa ser detectável
    }

    // Chamado pela HitBoxArea3D ao acertar. Repassa via sinal.
    public void ReceiveHit(HitBoxArea3D hitBox)
    {
        EmitSignal(SignalName.Hurt, hitBox);
    }
}
