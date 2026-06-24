using Godot;
using RPGFramework.Resources;

namespace RPGFramework.Core;

public partial class DefaultControllerAnimation
{
    [ExportGroup("Combat Animations")]
    [Export] public string[] CombatRecoveryAnimations { get; set; } =
        { "Sword_Regular_A_Rec", "Sword_Regular_B_Rec", "Sword_Regular_C" };
    [Export] public string HeavyComboAnimationName { get; set; } = "Sword_Heavy_Combo";
    // Hitbox do golpe — ligado/desligado por código conforme o estado de ataque da CombatSM.
    [Export] public HitBoxArea3D HitBox { get; set; }
    [Export] public WeaponComponent Weapon { get; set; } // arma equipada (dano por golpe)
    [Export] public string[] AttackAnimations { get; set; } =
        { "Sword_Regular_A", "Sword_Regular_B", "Sword_Regular_C" };
    // Player = true (lê input). IA/inimigo = false (ataca via RequestAttack do BT).
    [Export] public bool ListenToInput { get; set; } = true;

    private bool _attackJustPressed = false;
    private bool _heavyAttackJustPressed = false;
    private StringName _activeAttackNode = "";

    private void SetupCombatCallbacks()
    {
        // Ao terminar o último golpe/recovery, o OneShot faz fadeout de volta para a base.
        foreach (var anim in CombatRecoveryAnimations)
            OnAnimFinished(anim, () =>
            {
                AnimTree.Set(CombatOneShotParam, (int)AnimationNodeOneShot.OneShotRequest.FadeOut);
                Pawn.State?.SetAction(PawnState.Action.None); // combate terminou
            });

        // Rede de segurança: se o heavy combo terminar (mesmo interrompido), garante que
        // movimento e rotação voltem ativos, evitando o jogador travado.
        OnAnimFinished(HeavyComboAnimationName, () =>
        {
            Pawn.SetMovementEnabled(true);
            (Pawn.RotateModel as RotateDirection3D)?.SetRotationEnabled(true);
        });
    }

    // Disparo de ataque por código (ex: IA via behavior tree). Mesmo efeito do input.
    public void RequestAttack()      => _attackJustPressed = true;
    public void RequestHeavyAttack() => _heavyAttackJustPressed = true;
    // _UnhandledInput é mais confiável que IsActionJustPressed em _PhysicsProcess
    public override void _UnhandledInput(InputEvent @event)
    {
        if (!ListenToInput) return; // inimigos não reagem ao input do player
        if (@event.IsActionPressed("attack"))
            RequestAttack();
        if (@event.IsActionPressed("heavy_attack"))
            RequestHeavyAttack();
        if (@event.IsActionPressed("draw_weapon"))
            Weapon?.ToggleEquip(); // saca/guarda via WeaponComponent (modelo + library + estado)
    }

    private void UpdateCombat()
    {
        if (_combatSMPlayback == null) return;

        UpdateHitBox();

        // Heavy combo (tecla E): OneShot fire-and-forget, toca o clip uma vez e volta sozinho.
        if (_heavyAttackJustPressed)
        {
            _heavyAttackJustPressed = false;
            AnimTree.Set(HeavyOneShotParam, (int)AnimationNodeOneShot.OneShotRequest.Fire);
        }

        if (!_attackJustPressed) return;
        _attackJustPressed = false;

        StringName current = _combatSMPlayback.GetCurrentNode();

        // Janela de combo só durante o golpe ativo (A→B, B→C).
        if (current == "Sword_Regular_A")
            _combatSMPlayback.Travel("Sword_Regular_B");
        else if (current == "Sword_Regular_B")
            _combatSMPlayback.Travel("Sword_Regular_C");
        else
        {
            // Início fresco (idle/recovery/C): reseta a SM em A e dispara o OneShot (fadein).
            _combatSMPlayback.Travel("Sword_Regular_A");
            AnimTree.Set(CombatOneShotParam, (int)AnimationNodeOneShot.OneShotRequest.Fire);
            Pawn.State?.SetAction(PawnState.Action.Attacking); // entrou em combate
        }
    }

    // Liga o hitbox enquanto a CombatSM está num golpe ativo (A/B/C), desliga no resto.
    // Só age na MUDANÇA de estado: ao entrar num golpe novo, `Enabled = true` reseta o dedup
    // (cada golpe acerta de novo); ao sair, desliga. Limpo e sem depender de track de animação.
    private void UpdateHitBox()
    {
        // Hitbox da arma equipada (vindo da prefab); fallback pro fixo (ex: desarmado).
        HitBoxArea3D hb = Weapon?.CurrentHitBox ?? HitBox;
        if (hb == null) return;

        StringName current = _combatSMPlayback.GetCurrentNode();
        bool isAttack = System.Array.IndexOf(AttackAnimations, current.ToString()) >= 0;

        if (isAttack)
        {
            if (current != _activeAttackNode)
            {
                _activeAttackNode = current;

                // Dano data-driven: cada golpe do combo usa o dano da arma equipada.
                int comboIndex = System.Array.IndexOf(AttackAnimations, current.ToString());
                if (Weapon?.CurrentWeapon != null && comboIndex >= 0)
                    hb.Damage = Weapon.CurrentWeapon.DamageForCombo(comboIndex);

                hb.Enabled = true; // novo golpe → dedup limpo + ativo
            }
        }
        else if (_activeAttackNode != "")
        {
            _activeAttackNode = "";
            hb.Enabled = false;
        }
    }
}
