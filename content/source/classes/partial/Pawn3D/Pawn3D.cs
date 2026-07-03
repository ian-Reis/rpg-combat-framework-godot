using Godot;
using RPGFramework.Core;
using RPGFramework.Resources;

namespace RPGFramework.Entities;

[GlobalClass]
public partial class Pawn3D : CharacterBody3D
{
    [ExportGroup("References Nodes")]
    [Export] public SpringArm3D       SpringArm   { get; set; }
    [Export] public RotateDirection3D RotateModel { get; set; }
    [Export] public WeaponComponent   WeaponComponent { get; set; }

    [ExportGroup("Identity")]
    [Export] public EntityFaction Faction { get; set; } = EntityFaction.NPC;

    [ExportGroup("Resources")]
    [Export] public PawnStats Stats { get; set; }
    [Export] public PawnState State { get; set; }

    // Nome do grupo desta facção (ex: "enemy"). A IA mira alvos por esse nome.
    public StringName FactionGroup => Faction.ToString().ToLower();

    [ExportGroup("Attack Lunge")]
    // Curva (X: tempo normalizado 0..1, Y: multiplicador de velocidade). Null = ease-out padrão.
    [Export] public Curve LungeCurve    { get; set; }
    [Export] public float LungeDuration { get; set; } = 0.25f;

    [ExportGroup("Crouch")]
    [Export] public CollisionShape3D Collider        { get; set; }
    [Export] public float CrouchHeight               { get; set; } = 1.0f;   // altura do colisor agachado
    [Export] public float CrouchSpeedMultiplier      { get; set; } = 0.45f;  // fator de velocidade agachado
    [Export] public float CrouchTransitionSpeed      { get; set; } = 12f;    // suavidade do colisor

    public Vector2 Motion { get; private set; }

    private Vector3 _slideVelocity = Vector3.Zero;

    private Vector3 velocity = Vector3.Zero;

    private Vector3 _lungeDir   = Vector3.Zero;
    private float   _lungeForce = 0f;
    private float   _lungeTime  = 0f;
    private bool    _lunging    = false;

    private CapsuleShape3D _capsule;
    private float _standHeight;
    private float _colliderBottomY; // pé fixo no chão ao redimensionar
    private float _currentHeight;

    private static readonly float ProjectGravity =
        ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready()
    {
        // Registra a entidade no grupo da sua facção (player/enemy/npc/pet) para a IA mirar.
        AddToGroup(FactionGroup);

        // Estado runtime por-instância (se atribuído um .tres compartilhado, duplica pra não vazar entre entidades).
        State = State != null ? (PawnState)State.Duplicate() : new PawnState();

        if (Collider?.Shape is CapsuleShape3D cap)
        {
            _capsule         = cap;
            _standHeight     = cap.Height;
            _colliderBottomY = Collider.Position.Y - _standHeight * 0.5f;
            _currentHeight   = _standHeight;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        // Morto = corpo congelado no lugar (sem gravidade/movimento), pra não cair sem colisor.
        if (State?.IsDead == true) return;

        float dt = (float)delta;
        velocity = Velocity;

        ApplyGravity(ref velocity, dt);
        
        Movement(ref velocity, dt);
        Jump(ref velocity);
        Crouch(dt);
        Slide(ref velocity, dt);
        
        ApplyLunge(ref velocity, dt);

        Velocity = velocity;
        MoveAndSlide();
        PushRigidBodies();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        State.IsAimed = IsInstanceValid(WeaponComponent) && 
                WeaponComponent.CurrentWeapon?.CurrentMode is WeaponMode.Pistol && 
                ReadAim() && 
                State.IsArmed;

    }
    // ===== Fonte de intenção — sobrescrevível por subclasses (ex: PawnAI3D usa IA, não input) =====

    // Direção de movimento desejada em world-space (horizontal). Vector3.Zero = parado.
    protected virtual Vector3 ReadMoveDirection()
    {
        // O gate de "pode mover" fica no HandleMovement (State.CanMove); aqui só lê o input.
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        Motion = inputDir;

        if (SpringArm == null || inputDir == Vector2.Zero)
            return Vector3.Zero;

        // Direção relativa à câmera (SpringArm).
        float yawRad = Mathf.DegToRad(SpringArm.RotationDegrees.Y);
        Vector3 forward = new(-Mathf.Sin(yawRad), 0f, -Mathf.Cos(yawRad));
        Vector3 right   = new( Mathf.Cos(yawRad), 0f, -Mathf.Sin(yawRad));
        return (forward * -inputDir.Y + right * inputDir.X).Normalized();
    }
    protected virtual bool ReadJump()   => Input.IsActionJustPressed("jump");
    protected virtual bool ReadCrouch() => Input.IsActionPressed("crouch");
    protected virtual bool ReadAim()    => Input.IsActionPressed("aim");
    protected virtual bool ReadSlide()  => Input.IsActionPressed("slide");

    public bool IsAttacking() => State?.CurrentAction == PawnState.Action.Attacking;
    
    public void SetMovementEnabled(bool enabled) {if (State != null) State.CanMove = enabled;}
    

    // API de força — chamada por Call Method Track na timeline do AnimationPlayer.
    // Inicia um lunge fluido na direção que o RotateModel encara (forward = -Z, plano horizontal).
    // 'force' = velocidade de pico (units/s), modulada pela LungeCurve ao longo de LungeDuration.
    public void AddAttackForce(float force)
    {
        if (RotateModel == null) return;

        Vector3 forward = -RotateModel.GlobalTransform.Basis.Z;
        forward.Y = 0f;
        if (forward.LengthSquared() < 0.0001f) return;

        _lungeDir   = forward.Normalized();
        _lungeForce = force;
        _lungeTime  = 0f;
        _lunging    = true;
    }

    private void ApplyLunge(ref Vector3 velocity, float dt)
    {
        if (!_lunging) return;

        _lungeTime += dt;
        float t = LungeDuration > 0f ? Mathf.Clamp(_lungeTime / LungeDuration, 0f, 1f) : 1f;

        // LungeCurve dá o controle total do feel; sem curva, ease-out suave (pico no início).
        float weight = LungeCurve?.SampleBaked(t) ?? (1f - t) * (1f - t);
        float speed  = _lungeForce * weight;

        velocity.X = _lungeDir.X * speed;
        velocity.Z = _lungeDir.Z * speed;

        if (t >= 1f)
            _lunging = false;
    }

    public Vector2 GetHorizontalSpeed() => new Vector2(Velocity.X, Velocity.Z);
}
