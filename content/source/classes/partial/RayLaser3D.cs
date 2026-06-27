using Godot;

/// <summary>
/// Laser sight para armas em Godot 4.7 C#.
///
/// SETUP:
///   1. Adicione um Node3D filho no cano da arma (ex: "BarrelTip").
///   2. Adicione este script como filho desse nó.
///   3. Rotacione o "BarrelTip" para que -Z aponte na direção do tiro.
///   4. Configure CollisionMask para ignorar o layer do próprio player.
///   5. (Opcional) Ative Glow no WorldEnvironment para o efeito brilhar.
///
/// USO EM CÓDIGO:
///   if (_laser.IsHitting)
///       ApplyDamage(_laser.HitCollider, _laser.HitPoint, _laser.HitNormal);
/// </summary>
[GlobalClass]
public partial class RayLaser3D : Node3D
{
    // -------------------------------------------------------------------------
    // Exports
    // -------------------------------------------------------------------------

    [ExportGroup("Laser")]
    [Export] public Color  LaserColor        { get; set; } = new Color(1f, 0.05f, 0.05f);
    [Export] public float  MaxDistance       { get; set; } = 50f;
    [Export] public float  BeamRadius        { get; set; } = 0.005f;
    [Export] public float  EmissionIntensity { get; set; } = 4f;

    [ExportGroup("Impact Dot")]
    [Export] public bool  ShowImpactDot  { get; set; } = true;
    [Export] public float ImpactDotScale { get; set; } = 5f;   // multiplicador sobre BeamRadius

    [ExportGroup("Direção")]
    /// <summary>
    /// Nó de referência para a direção do laser (-Z do nó escolhido).
    /// Útil quando o modelo/bone do cano tem rotação incorreta.
    /// Se nulo, usa o -Z do próprio WeaponLaser como fallback.
    /// </summary>
    [Export] public Node3D ZDirectionNode { get; set; }

    /// <summary>
    /// Offset de rotação em graus aplicado sobre a direção final do raio.
    /// Use para corrigir o eixo sem mexer no modelo ou nos nós da cena.
    /// Exemplo comum: (90, 0, 0) quando o cano aponta em +Y em vez de -Z.
    /// </summary>
    [Export] public Vector3 DirectionOffsetDegrees { get; set; } = Vector3.Zero;

    [ExportGroup("Colisão")]
    [Export(PropertyHint.Layers3DPhysics)]
    public uint CollisionMask { get; set; } = 1;

    // -------------------------------------------------------------------------
    // Hit info — use estes valores nos seus sistemas de dano/gameplay
    // -------------------------------------------------------------------------

    public bool        IsHitting    { get; private set; }
    public Vector3     HitPoint     { get; private set; }
    public Vector3     HitNormal    { get; private set; }
    public GodotObject HitCollider  { get; private set; }
    public float       HitDistance  { get; private set; }

    // -------------------------------------------------------------------------
    // Nós internos (reutilizados a cada frame — sem alocação em _PhysicsProcess)
    // -------------------------------------------------------------------------

    private MeshInstance3D     _beamInstance;
    private MeshInstance3D     _dotInstance;
    private CylinderMesh       _beamMesh;
    private StandardMaterial3D _beamMat;
    private StandardMaterial3D _dotMat;

    // =========================================================================
    // Ciclo de vida
    // =========================================================================

    public override void _Ready()
    {
        SetupBeam();
        if (ShowImpactDot) SetupImpactDot();
    }

    public override void _PhysicsProcess(double delta)
    {
        PerformRaycast();
    }

    public override void _ExitTree()
    {
        // _dotInstance fica na raiz, não é filho nosso — precisa de cleanup manual
        _dotInstance?.QueueFree();
    }

    // =========================================================================
    // API pública
    // =========================================================================

    /// <summary>Liga ou desliga o laser (beam + dot + processamento).</summary>
    public void SetEnabled(bool enabled)
    {
        _beamInstance.Visible = enabled;
        if (_dotInstance != null)
            _dotInstance.Visible = enabled && IsHitting;

        SetPhysicsProcess(enabled);
    }

    /// <summary>Muda a cor do laser em runtime (útil para skins/modos de arma).</summary>
    public void SetColor(Color color)
    {
        LaserColor = color;
        _beamMat.AlbedoColor = color;
        _beamMat.Emission    = color;
        if (_dotMat != null)
        {
            _dotMat.AlbedoColor = color;
            _dotMat.Emission    = color;
        }
    }

    // =========================================================================
    // Setup
    // =========================================================================

    private void SetupBeam()
    {
        // CylinderMesh reutilizado — só atualizamos Height a cada frame
        _beamMesh = new CylinderMesh
        {
            TopRadius      = BeamRadius,
            BottomRadius   = BeamRadius,
            Height         = 1f,          // atualizado em UpdateBeamTransform()
            RadialSegments = 6,
            Rings          = 1,
        };

        _beamMat = MakeLaserMaterial(LaserColor, EmissionIntensity);

        _beamInstance = new MeshInstance3D
        {
            Mesh             = _beamMesh,
            MaterialOverride = _beamMat,
            CastShadow       = GeometryInstance3D.ShadowCastingSetting.Off,
        };

        AddChild(_beamInstance);   // filho do WeaponLaser → se move com a arma
    }

    private void SetupImpactDot()
    {
        float dotRadius = BeamRadius * ImpactDotScale;

        var dotMesh = new SphereMesh
        {
            Radius         = dotRadius,
            Height         = dotRadius * 2f,
            RadialSegments = 8,
            Rings          = 4,
        };

        _dotMat = MakeLaserMaterial(LaserColor, EmissionIntensity * 1.5f);

        _dotInstance = new MeshInstance3D
        {
            Mesh             = dotMesh,
            MaterialOverride = _dotMat,
            CastShadow       = GeometryInstance3D.ShadowCastingSetting.Off,
            Visible          = false,
        };

        // Adicionado na RAIZ com CallDeferred: evita "Parent node is busy" quando
        // o laser é instanciado durante o _Ready() de outro nó (ex: WeaponComponent).
        GetTree().Root.CallDeferred(Node.MethodName.AddChild, _dotInstance);
    }

    private static StandardMaterial3D MakeLaserMaterial(Color color, float emission)
    {
        return new StandardMaterial3D
        {
            ShadingMode              = StandardMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoColor              = color,
            EmissionEnabled          = true,
            Emission                 = color,
            EmissionEnergyMultiplier = emission,
            CullMode                 = BaseMaterial3D.CullModeEnum.Disabled,
            // NoDepthTest = true → laser aparece SOBRE paredes (estilo ponto laser real)
            // NoDepthTest = false → laser fica bloqueado por geometria
            NoDepthTest              = false,
        };
    }

    // =========================================================================
    // Raycast + atualização visual
    // =========================================================================

    private void PerformRaycast()
    {
        Vector3 origin = GlobalPosition;

        // Usa o -Z do ZDirectionNode se estiver setado e válido; senão, -Z do próprio nó (fallback).
        Basis sourceBasis = (ZDirectionNode != null && IsInstanceValid(ZDirectionNode))
            ? ZDirectionNode.GlobalBasis
            : GlobalBasis;

        Vector3 direction = (-sourceBasis.Z).Normalized();

        // Aplica offset de rotação em graus (em espaço local do nó de referência).
        // Permite corrigir eixo errado do modelo sem alterar a cena.
        if (DirectionOffsetDegrees != Vector3.Zero)
        {
            var offsetBasis = new Basis(
                Quaternion.FromEuler(DirectionOffsetDegrees * (Mathf.Pi / 180f))
            );
            // Rotaciona o vetor de direção no espaço global do nó fonte
            direction = (sourceBasis * offsetBasis * sourceBasis.Inverse() * direction).Normalized();
        }

        var spaceState = GetWorld3D().DirectSpaceState;
        var query = PhysicsRayQueryParameters3D.Create(
            from:          origin,
            to:            origin + direction * MaxDistance,
            collisionMask: CollisionMask
        );

        var result = spaceState.IntersectRay(query);

        Vector3 endPoint;

        if (result.Count > 0)
        {
            endPoint    = result["position"].AsVector3();
            HitPoint    = endPoint;
            HitNormal   = result["normal"].AsVector3();
            HitCollider = result["collider"].AsGodotObject();
            HitDistance = origin.DistanceTo(endPoint);
            IsHitting   = true;

            if (_dotInstance != null)
            {
                // Offset mínimo pela normal para evitar z-fighting
                _dotInstance.GlobalPosition = HitPoint + HitNormal * 0.005f;
                _dotInstance.Visible        = true;
            }
        }
        else
        {
            endPoint    = origin + direction * MaxDistance;
            IsHitting   = false;
            HitCollider = null;
            HitDistance = MaxDistance;

            if (_dotInstance != null)
                _dotInstance.Visible = false;
        }

        UpdateBeamTransform(origin, endPoint);
    }

    private void UpdateBeamTransform(Vector3 from, Vector3 to)
    {
        float distance = from.DistanceTo(to);

        if (distance < 0.001f)
        {
            _beamInstance.Visible = false;
            return;
        }

        _beamInstance.Visible = true;
        _beamMesh.Height      = distance;

        Vector3 dir      = (to - from).Normalized();
        Vector3 midPoint = (from + to) * 0.5f;

        // CylinderMesh tem altura no eixo Y local.
        // Construímos uma Basis onde Y aponta na direção do raio.
        Vector3 refAxis = Mathf.Abs(dir.Dot(Vector3.Up)) > 0.99f
            ? Vector3.Forward   // evita degenerar o cross product quando dir ≈ Up
            : Vector3.Up;

        Vector3 right   = dir.Cross(refAxis).Normalized();
        Vector3 forward = right.Cross(dir).Normalized();

        _beamInstance.GlobalTransform = new Transform3D(
            new Basis(right, dir, -forward),
            midPoint
        );
    }
}