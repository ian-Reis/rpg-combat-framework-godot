using System.Collections.Generic;
using Godot;

namespace RPGFramework.Core;

[GlobalClass]
public partial class HitBoxArea3D : Area3D
{
    // Emitido quando acerta uma hurtbox.
    [Signal] public delegate void HitEventHandler(HurtBoxArea3D hurtBox);

    // Layer 3 = hitbox, detecta layer 2 = hurtbox (ver CLAUDE.md).
    private const uint HitboxLayer  = 1u << 2;
    private const uint HurtboxLayer = 1u << 1;

    [Export] public float Damage { get; set; } = 10f;
    // Origem do golpe (ex: o Pawn atacante) — evita acertar a própria entidade.
    [Export] public Node3D Source { get; set; }

    private bool _enabled;

    // Propriedade reativa: pode ser dirigida por um Value Track na animação OU por código.
    // Ligar limpa a lista de acertos (cada golpe pode acertar de novo) e ativa o monitoramento.
    [Export] public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            if (value)
                _alreadyHit.Clear();

            if (IsInsideTree())
            {
                Monitoring = value;
                GD.Print($"[HitBox:{Name}] Enabled = {value}");
            }
        }
    }

    // Evita acertar a mesma hurtbox mais de uma vez no mesmo golpe.
    private readonly HashSet<HurtBoxArea3D> _alreadyHit = new();

    public override void _Ready()
    {
        CollisionLayer = HitboxLayer;
        CollisionMask  = HurtboxLayer;
        AreaEntered   += OnAreaEntered;
        Monitoring     = _enabled; // aplica o estado inicial da propriedade
    }

    private void OnAreaEntered(Area3D area)
    {
        if (area is not HurtBoxArea3D hurtBox) return;
        if (Source != null && hurtBox.Entity == Source)
        {
            GD.Print($"[HitBox:{Name}] ignorou o próprio dono ({Source.Name})");
            return; // não acerta o próprio dono
        }
        if (!_alreadyHit.Add(hurtBox))
        {
            GD.Print($"[HitBox:{Name}] {hurtBox.Name} já foi acertado neste golpe");
            return; // já acertou neste golpe
        }

        GD.Print($"[HitBox:{Name}] ACERTOU {hurtBox.Name} (dano={Damage})");
        hurtBox.ReceiveHit(this);
        EmitSignal(SignalName.Hit, hurtBox);
    }

    // Atalhos para Call Method Track / código — todos passam pela propriedade reativa.
    public void Enable()  => Enabled = true;
    public void Disable() => Enabled = false;
    public void SetActive(bool active) => Enabled = active;

    // Desativa de forma deferida — seguro mesmo chamado de dentro de um sinal de física (ex: morte).
    public void DisableDeferred()
    {
        _enabled = false;
        SetDeferred(Area3D.PropertyName.Monitoring, false);
    }
}
