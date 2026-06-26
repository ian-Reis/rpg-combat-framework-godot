using Godot;
using RPGFramework.Core;
using RPGFramework.Resources;

namespace RPGFramework.Entitys;

// Pawn dirigido por IA (NPCs e inimigos). Reaproveita todo o movimento/gravidade/lunge/crouch
// do Pawn3D, mas troca a fonte de input por comandos da IA (behavior tree, etc).
// O RotateModel deve ser um Node3D comum (NÃO um RotateDirection3D, que lê input).
[GlobalClass]
public partial class PawnAI3D : Pawn3D
{
    [ExportGroup("AI References")]
    [Export] public AnimationController AnimController { get; set; }
    [Export] public Node BTPlayer { get; set; }            // LimboAI BTPlayer (Node, sem tipo C#)
    [Export] public HurtBoxArea3D HurtBox { get; set; }

    [ExportGroup("AI Facing")]
    [Export] public float FacingSpeed { get; set; } = 10f; // suavidade do giro do modelo

    private Vector3 _moveDir = Vector3.Zero;  // direção de movimento desejada (world)
    private Vector3 _faceDir = Vector3.Zero;  // direção para o modelo encarar
    private bool    _jumpRequested;
    private bool    _crouching;

    // ===== Intenção (sobrescreve a fonte de input do Pawn3D) =====
    protected override Vector3 ReadMoveDirection()
    {
        Vector3 dir = _moveDir;
        dir.Y = 0f;
        return dir.LengthSquared() > 1f ? dir.Normalized() : dir;
    }

    protected override bool ReadJump()
    {
        if (!_jumpRequested) return false;
        _jumpRequested = false; // one-shot
        return true;
    }

    protected override bool ReadCrouch() => _crouching;

    public override void _Ready()
    {
        base._Ready();
        // Reage à morte (estado setado pelo AnimationController ao zerar a vida).
        State.StateChanged += OnStateChanged;
    }

    private void OnStateChanged()
    {
        if (State.CurrentAction == PawnState.Action.Dead)
            OnDeath();
    }

    // Ao morrer: para a IA, desativa a hurtbox e o colisor do corpo (player atravessa o cadáver).
    private void OnDeath()
    {
        StopMoving();
        BTPlayer?.Set("active", false);
        HurtBox?.SetActive(false);
        // deferred: pode rodar de dentro de um sinal de física (area_entered → morte).
        Collider?.SetDeferred(CollisionShape3D.PropertyName.Disabled, true);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        UpdateFacing((float)delta);
    }

    // Gira o RotateModel suavemente na direção de _faceDir (mesma convenção do RotateDirection3D).
    private void UpdateFacing(float dt)
    {
        if (RotateModel == null || _faceDir.LengthSquared() < 0.0001f) return;

        float targetYaw = Mathf.Atan2(_faceDir.X, _faceDir.Z);
        Vector3 rot = RotateModel.Rotation;
        rot.Y = Mathf.LerpAngle(rot.Y, targetYaw, 1f - Mathf.Exp(-FacingSpeed * dt));
        RotateModel.Rotation = rot;
    }

    // ===== API para a IA / BT tasks =====

    // Define a direção de movimento em world-space (ex: vinda de um pathfinding).
    public void SetMoveDirection(Vector3 worldDir)
    {
        _moveDir = worldDir;
        worldDir.Y = 0f;
        if (worldDir.LengthSquared() > 0.0001f) _faceDir = worldDir;
    }

    // Move em direção a um node (atalho comum para "perseguir o alvo").
    public void MoveTowards(Node3D target)
    {
        if (target == null) { StopMoving(); return; }
        SetMoveDirection(target.GlobalPosition - GlobalPosition);
    }

    public void StopMoving() => _moveDir = Vector3.Zero;

    // Encara o alvo sem se mover (ex: ao atacar parado em alcance).
    public void FaceTowards(Node3D target)
    {
        if (target == null) return;
        Vector3 d = target.GlobalPosition - GlobalPosition;
        d.Y = 0f;
        if (d.LengthSquared() > 0.0001f) _faceDir = d;
    }

    public void RequestJump()          => _jumpRequested = true;
    public void SetCrouching(bool on)  => _crouching = on;

    // Hook de ataque — chamado pela task `attack` do BT.
    // Reaproveita a mesma animação de combate do player via AnimationController.
    // O estado (Attacking/None) é gerido pelo combate, não aqui.
    public virtual void Attack()
    {
        if (AnimController == null)
        {
            GD.PrintErr($"[PawnAI3D:{Name}] Attack() chamado mas AnimController não está atribuído no inspector!");
            return;
        }
        GD.Print($"[PawnAI3D:{Name}] Attack() → RequestAttack");
        AnimController.RequestAttack();
    }
}
