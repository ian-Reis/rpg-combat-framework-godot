using Godot;

namespace RPGFramework.Core;

public partial class DefaultControllerAnimation
{
    [ExportGroup("Combat Animations")]
    [Export] public string[] CombatRecoveryAnimations { get; set; } =
        { "Sword_Regular_A_Rec", "Sword_Regular_B_Rec", "Sword_Regular_C" };

    private bool _attackJustPressed = false;

    private void SetupCombatCallbacks()
    {
        // Ao terminar o último golpe/recovery, o OneShot faz fadeout de volta para a base.
        foreach (var anim in CombatRecoveryAnimations)
            OnAnimFinished(anim, () =>
                AnimTree.Set(CombatOneShotParam, (int)AnimationNodeOneShot.OneShotRequest.FadeOut));
    }

    // _UnhandledInput é mais confiável que IsActionJustPressed em _PhysicsProcess
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("attack"))
            _attackJustPressed = true;
    }

    private void UpdateCombat()
    {
        if (_combatSMPlayback == null || !_attackJustPressed) return;
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
