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
    [Export] public Action CurrentAction { get; set; } = Action.None;
    
    [ExportGroup("Can Do?")]
    [Export] public bool CanMove { get; set; } = true;
    [Export] public bool CanJump { get; set; } = true;
    [Export] public bool CanAttack { get; set; } = true;
    [Export] public bool CanCrouch { get; set; } = true;
    [Export] public bool CanSlide { get; set; } = true;
    [Export] public bool CanRoll { get; set; } = true;
    [Export] public bool CanCutTree { get; set; } = true;
    

    [ExportGroup("How is Doing?")]
    [Export] public bool IsArmed { get; set; } = false;
    [Export] public bool IsAimed { get; set; } = false;
    [Export] public bool IsSliding { get; set; } = false;
    [Export] public bool IsRolling { get; set; } = false;
    [Export] public bool IsCutting { get; set; } = false;
    [Export] public bool IsCrouching { get; set; } = false;

    public bool IsBusy => CurrentAction != Action.None;
    public bool IsDead => CurrentAction == Action.Dead;

    public void SetArmed(bool armed)
    {
        if (IsArmed == armed) return;
        GD.Print($"[PawnState] armado: {armed}");
        IsArmed = armed;
        EmitSignal(SignalName.StateChanged);
    }

    public void SetAimed(bool aimed)
    {
        if (IsAimed == aimed) return;
        GD.Print($"[PawnState] mirado: {aimed}");
        IsAimed = aimed;
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
