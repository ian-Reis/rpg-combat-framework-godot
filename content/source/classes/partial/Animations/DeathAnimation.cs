using Godot;
using RPGFramework.Resources;

namespace RPGFramework.Core;

public partial class AnimationController
{
    // Troca pro estado "Dead" (Transition no topo) e congela o resto da animação.
    public void Die()
    {
        if (_dead) return;
        _dead = true;
        HitBox?.DisableDeferred(); // inimigo morto não dá mais dano (mesmo se morreu no meio do golpe)
        Pawn?.State?.SetAction(PawnState.Action.Dead);
        // AnimTree.Set(DeathTransitionParam, DeathState);
        if (Parameters.TransitionRequest.TryGetValue("dead", out var deadTRPath))
            AnimTree.Set(deadTRPath, "Dead");
    }
}
