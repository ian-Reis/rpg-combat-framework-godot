using Godot;
using RPGFramework.Core;
using RPGFramework.Resources;

namespace RPGFramework.Entitys;

[GlobalClass]
public partial class Pawn3D : CharacterBody3D
{
    [ExportGroup("References Nodes")]
    [Export] public SpringArm3D SpringArm  { get; set; }
    [Export] public Node3D      RotateModel { get; set; }

    [ExportGroup("Resources")]
    [Export] public PawnStats Stats { get; set; }

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
    public bool    IsCrouching { get; private set; }

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
        float dt = (float)delta;
        Vector3 velocity = Velocity;

        UpdateCrouch(dt);
        ApplyGravity(ref velocity, dt);
        HandleJump(ref velocity);
        HandleMovement(ref velocity, dt);
        ApplyLunge(ref velocity, dt);

        Velocity = velocity;
        MoveAndSlide();
        PushRigidBodies();
    }

    private void ApplyGravity(ref Vector3 velocity, float dt)
    {
        if (!IsOnFloor())
            velocity.Y -= ProjectGravity * (Stats?.GravityScale ?? 1f) * dt;
    }

    private void HandleJump(ref Vector3 velocity)
    {
        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
            velocity.Y = Stats?.JumpForce ?? 5f;
    }

    private void HandleMovement(ref Vector3 velocity, float dt)
    {
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        Motion = inputDir;

        Vector3 moveDir = Vector3.Zero;
        if (SpringArm != null && inputDir != Vector2.Zero)
        {
            float yawRad = Mathf.DegToRad(SpringArm.RotationDegrees.Y);
            Vector3 forward = new(-Mathf.Sin(yawRad), 0f, -Mathf.Cos(yawRad));
            Vector3 right   = new( Mathf.Cos(yawRad), 0f, -Mathf.Sin(yawRad));
            moveDir = (forward * -inputDir.Y + right * inputDir.X).Normalized();
        }

        float speed = Stats?.Speed ?? 5f;
        if (IsCrouching) speed *= CrouchSpeedMultiplier;
        float accel = Stats?.Acceleration ?? 15f;
        float fric  = Stats?.Friction ?? 10f;

        float targetX = moveDir.X * speed;
        float targetZ = moveDir.Z * speed;

        if (moveDir != Vector3.Zero)
        {
            float t = 1f - Mathf.Exp(-accel * dt);
            velocity.X = Mathf.Lerp(velocity.X, targetX, t);
            velocity.Z = Mathf.Lerp(velocity.Z, targetZ, t);
        }
        else
        {
            float t = 1f - Mathf.Exp(-fric * dt);
            velocity.X = Mathf.Lerp(velocity.X, 0f, t);
            velocity.Z = Mathf.Lerp(velocity.Z, 0f, t);
        }
    }

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

    private void UpdateCrouch(float dt)
    {
        // Hold: agacha só no chão. Solta = levanta.
        IsCrouching = IsOnFloor() && Input.IsActionPressed("crouch");

        if (_capsule == null) return;

        // Redimensiona o colisor suavemente mantendo os pés no chão.
        float target = IsCrouching ? CrouchHeight : _standHeight;
        _currentHeight = Mathf.Lerp(_currentHeight, target, 1f - Mathf.Exp(-CrouchTransitionSpeed * dt));
        _capsule.Height = _currentHeight;

        Vector3 p = Collider.Position;
        p.Y = _colliderBottomY + _currentHeight * 0.5f;
        Collider.Position = p;
    }

    private void PushRigidBodies()
    {
        float pushForce = Stats?.PushForce ?? 5f;
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision3D col = GetSlideCollision(i);
            if (col.GetCollider() is RigidBody3D rb)
                rb.ApplyImpulse(-col.GetNormal() * pushForce, col.GetPosition() - rb.GlobalPosition);
        }
    }
}
