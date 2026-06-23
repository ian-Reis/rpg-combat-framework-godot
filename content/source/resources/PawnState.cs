using Godot;

namespace RPGFramework.Resources;

// Blackboard de estado runtime do Pawn. Vários sistemas leem/escrevem:
// animação escolhe moveset, input bloqueia ações, hitbox checa se pode acertar, etc.
[GlobalClass]
public partial class PawnState : Resource
{
    public enum Weapon { Unarmed, Sword }
    public enum Action { None, Attacking, Blocking, Dodging, Hurt, Dead }

    // Emitido sempre que arma ou ação mudam (Resource.Changed é reservado, por isso nome próprio).
    [Signal] public delegate void StateChangedEventHandler();

    [Export] public Weapon CurrentWeapon { get; set; } = Weapon.Unarmed;
    [Export] public Action CurrentAction { get; set; } = Action.None;

    public bool IsArmed => CurrentWeapon != Weapon.Unarmed;
    public bool IsBusy  => CurrentAction != Action.None;
    public bool IsDead  => CurrentAction == Action.Dead;

    public void SetWeapon(Weapon weapon)
    {
        if (CurrentWeapon == weapon) return;
        GD.Print($"[PawnState] arma: {CurrentWeapon} → {weapon}");
        CurrentWeapon = weapon;
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
