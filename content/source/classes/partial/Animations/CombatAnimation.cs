using Godot;

namespace RPGFramework.Core;

public partial class DefaultControllerAnimation
{
    [ExportGroup("Combat Animations")]
    [Export] public string[] CombatRecoveryAnimations { get; set; } =
        { "Sword_Regular_A_Rec", "Sword_Regular_B_Rec", "Sword_Regular_C" };

    private bool  _attackJustPressed = false;
    private float _combatBlend       = 0f;
    private bool  _isCombatActive    = false;

    private void SetupCombatCallbacks()
    {
        foreach (var anim in CombatRecoveryAnimations)
            OnAnimFinished(anim, () => _isCombatActive = false);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("attack"))
            _attackJustPressed = true;
    }

    private void UpdateCombat(float dt)
    {
        if (_combatSMPlayback == null) return;

        if (_attackJustPressed)
        {
            StringName current = _combatSMPlayback.GetCurrentNode();
            _isCombatActive = true;

            if (current == "Sword_Regular_A")
                _combatSMPlayback.Travel("Sword_Regular_B");
            else if (current == "Sword_Regular_B")
                _combatSMPlayback.Travel("Sword_Regular_C");
            else
                _combatSMPlayback.Start("Sword_Regular_A");

            _attackJustPressed = false;
        }

        float target = _isCombatActive ? 1f : 0f;
        _combatBlend = Mathf.Lerp(_combatBlend, target, 1f - Mathf.Exp(-CombatBlendSpeed * dt));
        AnimTree.Set(CombatBlendParam, _combatBlend);
    }
}
