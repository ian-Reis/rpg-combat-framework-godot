using Godot;
using Animation;
using Components;
using Helpers;

namespace Animation;

[GlobalClass]
public partial class AnimStateAttack : AnimationState
{
    // Name of the next attack state in the combo chain. Leave empty to end the combo.
    [Export] public string NextComboState = "";

    // Normalized position (0..1) from which the next attack input is accepted.
    [Export] public float ComboWindowStart = 0.55f;

    // Seconds before the end to trigger finish detection. Increase for longer animations.
    [Export] public float EndThreshold = 0.1f;

    private bool _comboQueued;

    public override void Enter(AnimationStateMachineComponent sm)
    {
        _comboQueued = false;
        AnimationTreeHelper.Travel(sm, StateName);
    }

    public override void Update(AnimationStateMachineComponent sm, AnimationSnapshot snapshot, float delta)
    {
        if (!_comboQueued && CanChain(sm) && Input.IsActionJustPressed("attack"))
            _comboQueued = true;

        if (!AnimationTreeHelper.AnimationFinish(sm, EndThreshold)) return;

        if (_comboQueued && !string.IsNullOrEmpty(NextComboState))
            sm.ChangeState(NextComboState);
        else
            sm.ChangeState("locomotion");
    }

    public override void Exit(AnimationStateMachineComponent sm)
    {
        _comboQueued = false;
    }

    private bool CanChain(AnimationStateMachineComponent sm)
        => !string.IsNullOrEmpty(NextComboState)
        && AnimationTreeHelper.GetNormalizedPlayPosition(sm) >= ComboWindowStart;
}
