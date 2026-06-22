using Godot;

namespace RPGFramework.Core;

public partial class DefaultControllerAnimation
{
    [ExportGroup("Combat Animations")]
    [Export] public string[] CombatRecoveryAnimations { get; set; } =
        { "Sword_Regular_A_Rec", "Sword_Regular_B_Rec", "Sword_Regular_C" };
    [Export] public string HeavyComboAnimationName { get; set; } = "Sword_Heavy_Combo";

    private bool _attackJustPressed = false;
    private bool _heavyAttackJustPressed = false;

    private void SetupCombatCallbacks()
    {
        // Ao terminar o último golpe/recovery, o OneShot faz fadeout de volta para a base.
        foreach (var anim in CombatRecoveryAnimations)
            OnAnimFinished(anim, () =>
                AnimTree.Set(CombatOneShotParam, (int)AnimationNodeOneShot.OneShotRequest.FadeOut));

        // Rede de segurança: se o heavy combo terminar (mesmo interrompido), garante que
        // movimento e rotação voltem ativos, evitando o jogador travado.
        OnAnimFinished(HeavyComboAnimationName, () =>
        {
            Pawn.SetMovementEnabled(true);
            (Pawn.RotateModel as RotateDirection3D)?.SetRotationEnabled(true);
        });
    }

    // _UnhandledInput é mais confiável que IsActionJustPressed em _PhysicsProcess
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("attack"))
            _attackJustPressed = true;
        if (@event.IsActionPressed("heavy_attack"))
            _heavyAttackJustPressed = true;
    }

    private void UpdateCombat()
    {
        if (_combatSMPlayback == null) return;

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
        }
    }
}
