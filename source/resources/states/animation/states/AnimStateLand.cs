using Components;
using Godot;
using Helpers;

namespace Animation;

[GlobalClass]
public partial class AnimStateLand : AnimationState
{
    // Tempo mínimo em ar para considerar aterrissagem pesada
    [Export] public float HardLandThreshold = 1.2f;

    public override void Enter(AnimationStateMachineComponent sm)
    {
        // Poderia ativar uma condição de land no AnimationTree aqui
        // Por enquanto, volta imediatamente para locomotion após o frame de impacto
    }

    public override void Update(AnimationStateMachineComponent sm, AnimationSnapshot snapshot, float delta)
    {
        // AnimationTree drives the "At End" transition to locomotion — just observe when it happened.
        if (AnimationTreeHelper.GetCurrentNode(sm) != StateName)
            sm.ChangeState("locomotion");
    }
}
