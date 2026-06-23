using System.Collections.Generic;
using Godot;

namespace RPGFramework.Core;

[GlobalClass]
public partial class HitBoxArea3D : Area3D
{
    // Emitido quando acerta uma hurtbox.
    [Signal] public delegate void HitEventHandler(HurtBoxArea3D hurtBox);

    // Layer 3 = hitbox, detecta layer 2 = hurtbox (ver CLAUDE.md). Ajustável no inspector.
    private const uint HitboxLayer  = 1u << 2;
    private const uint HurtboxLayer = 1u << 1;

    [Export] public float Damage { get; set; } = 10f;
    // Origem do golpe (ex: o Pawn atacante) — evita acertar a própria entidade.
    [Export] public Node3D Source { get; set; }

    // Evita acertar a mesma hurtbox mais de uma vez no mesmo golpe.
    private readonly HashSet<HurtBoxArea3D> _alreadyHit = new();

    public override void _Ready()
    {
        CollisionLayer = HitboxLayer;
        CollisionMask  = HurtboxLayer;
        Monitoring     = false; // ativa só nos frames ativos do golpe (via Enable)
        AreaEntered   += OnAreaEntered;
    }

    private void OnAreaEntered(Area3D area)
    {
        if (area is not HurtBoxArea3D hurtBox) return;
        if (Source != null && hurtBox.Entity == Source) return; // não acerta o próprio dono
        if (!_alreadyHit.Add(hurtBox)) return;                   // já acertou neste golpe

        hurtBox.ReceiveHit(this);
        EmitSignal(SignalName.Hit, hurtBox);
    }

    // API (Call Method Track / código): liga o hitbox nos frames ativos do golpe.
    // Limpa a lista de acertos para que cada golpe possa acertar de novo.
    public void Enable()
    {
        _alreadyHit.Clear();
        Monitoring = true;
    }

    public void Disable() => Monitoring = false;

    public void SetActive(bool active)
    {
        if (active) Enable();
        else Disable();
    }
}
