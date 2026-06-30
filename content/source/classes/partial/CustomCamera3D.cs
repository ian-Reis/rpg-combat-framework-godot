using Godot;
using RPGFramework.Entities;

namespace RPGFramework.Core;

[GlobalClass]
public partial class CustomCamera3D : Camera3D
{
    [ExportGroup("Trauma Shake")]
    [Export] public float MaxPitchShake { get; set; } = 5f;
    [Export] public float MaxYawShake   { get; set; } = 5f;
    [Export] public float MaxRollShake  { get; set; } = 3f;
    [Export] public float TraumaDecay   { get; set; } = 1.5f;

    [ExportGroup("Dynamic FOV")]
    [Export] public CharacterBody3D Character    { get; set; }
    [Export] public float           BaseFov      { get; set; } = 75f;
    [Export] public float           MaxFovBoost  { get; set; } = 15f;
    [Export] public float           MaxSpeed     { get; set; } = 10f;
    [Export] public float           FovSmoothing { get; set; } = 8f;
    [Export] public float           AimedFov     { get; set; } = 40f;

    [ExportGroup("Strafe Tilt")]
    [Export] public float MaxTilt       { get; set; } = 3f;
    [Export] public float TiltSmoothing { get; set; } = 8f;

    private float _trauma;
    private float _tiltCurrent;
    private float _fovAmount;

    /// <summary> Adicione trauma de 0 a 1; acumula até o máximo de 1. </summary>
    public void AddTrauma(float amount) =>
        _trauma = Mathf.Min(_trauma + amount, 1f);

    public override void _Ready() => Fov = BaseFov;

    public override void _Process(double delta)
    {
        float dt   = (float)delta;
        float time = (float)Time.GetTicksMsec() / 1000f;

        // Trauma shake — intensidade quadrática para queda suave
        _trauma = Mathf.Max(_trauma - TraumaDecay * dt, 0f);
        float shake = _trauma * _trauma;

        float pitchShake = MaxPitchShake * shake * Mathf.Sin(time * 37f);
        float yawShake   = MaxYawShake   * shake * Mathf.Sin(time * 29f);
        float rollShake  = MaxRollShake  * shake * Mathf.Sin(time * 53f);

        // Strafe tilt
        float inputX     = Input.GetAxis("move_left", "move_right");
        float t          = 1f - Mathf.Exp(-TiltSmoothing * dt);
        _tiltCurrent     = Mathf.Lerp(_tiltCurrent, -inputX * MaxTilt, t);

        RotationDegrees = new Vector3(pitchShake, yawShake, rollShake + _tiltCurrent);

        // FOV dinâmico por velocidade
        if (Character == null) return;

        // float speedRatio = Mathf.Clamp(Character.Velocity.Length() / MaxSpeed, 0f, 1f);
        // float speedRatioFovT       = 1f - Mathf.Exp(-FovSmoothing * dt);
        
// 1. Calcula o FOV base desejado (Muda para AimedFov se estiver mirando, senão usa o BaseFov)
        Pawn3D pawn = (Pawn3D)Character;
        float targetFov = pawn.State.IsAimed ? AimedFov : BaseFov;

        // 2. Adiciona o boost de velocidade apenas se NÃO estiver mirando (opcional, ajuste conforme seu design)
        if (!pawn.State.IsAimed)
        {
            float speedRatio = Mathf.Clamp(Character.Velocity.Length() / MaxSpeed, 0f, 1f);
            targetFov += MaxFovBoost * speedRatio;
        }

        // 3. Suavização correta e independente da taxa de quadros (Framerate Independent Lerp)
        float fovT = 1f - Mathf.Exp(-FovSmoothing * dt);
        Fov = Mathf.Lerp(Fov, targetFov, fovT);
    }
}
