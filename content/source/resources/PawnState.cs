using Godot;

namespace RPGFramework.Resources;

// Blackboard de estado runtime do Pawn. Vários sistemas leem/escrevem:
// animação escolhe pose, input bloqueia ações, hitbox checa se pode acertar, etc.
[GlobalClass]
public partial class PawnState : Resource
{
    public enum Action { None, Attacking, Blocking, Dodging, Hurt, Dead }

    [Signal] public delegate void StateChangedEventHandler();

    // Armado com QUALQUER arma. A identidade real da arma é o WeaponResource (WeaponComponent).
    [Export] public bool IsArmed { get; set; }
    [Export] public Action CurrentAction { get; set; } = Action.None;

    public bool IsBusy => CurrentAction != Action.None;
    public bool IsDead => CurrentAction == Action.Dead;

    public void SetArmed(bool armed)
    {
        if (IsArmed == armed) return;
        GD.Print($"[PawnState] armado: {armed}");
        IsArmed = armed;
        EmitSignal(SignalName.StateChanged);
    }

    public void SetAction(Action action)
    {
        if (CurrentAction == action) return;
        GD.Print($"[PawnState] ação: {CurrentAction} → {action}");
        CurrentAction = action;
        EmitSignal(SignalName.StateChanged);
    }
}
