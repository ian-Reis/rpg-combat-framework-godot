using Godot;
using System.Collections.Generic;

/// <summary>
/// Utilitário de debug visual estilo Unreal Engine.
/// Adicione como Autoload ou filho de um nó persistente na cena.
/// </summary>
[GlobalClass]
public partial class DebugVisual : Node
{
    private static DebugVisual _instance;
    private readonly List<DebugShape> _activeShapes = new();

    private struct DebugShape
    {
        public MeshInstance3D MeshInstance;
        public float RemainingTime;
    }

    public override void _Ready()
    {
        _instance = this;
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Process(double delta)
    {
        for (int i = _activeShapes.Count - 1; i >= 0; i--)
        {
            var shape = _activeShapes[i];
            shape.RemainingTime -= (float)delta;

            if (shape.RemainingTime <= 0f)
            {
                shape.MeshInstance.QueueFree();
                _activeShapes.RemoveAt(i);
            }
            else
            {
                _activeShapes[i] = shape;
            }
        }
    }

    // -------------------------------------------------------------------------
    // MÉTODOS ESTÁTICOS PÚBLICOS (ESTILO UNREAL)
    // -------------------------------------------------------------------------

    /// <summary>Desenha uma linha entre dois pontos no espaço 3D.</summary>
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0f)
    {
        if (_instance == null) return;

        var mesh = new ImmediateMesh();
        mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
        mesh.SurfaceSetColor(color);
        mesh.SurfaceAddVertex(start);
        mesh.SurfaceSetColor(color);
        mesh.SurfaceAddVertex(end);
        mesh.SurfaceEnd();

        // ImmediateMesh define cor por vértice: VertexColorUseAsAlbedo = true
        _instance.CreateShapeInstance(mesh, duration, color, vertexColor: true);
    }

    /// <summary>Desenha um raio a partir de uma origem em uma direção.</summary>
    public static void DrawRay(Vector3 origin, Vector3 direction, float length, Color color, float duration = 0f)
    {
        DrawLine(origin, origin + direction.Normalized() * length, color, duration);
    }

    /// <summary>Desenha uma esfera sólida de debug (útil para pontos de impacto).</summary>
    public static void DrawSphere(Vector3 position, float radius, Color color, float duration = 0f)
    {
        if (_instance == null) return;

        var mesh = new SphereMesh
        {
            Radius = radius,
            Height = radius * 2f,
            RadialSegments = 8,
            Rings = 4,
        };

        var node = _instance.CreateShapeInstance(mesh, duration, color);
        node.GlobalPosition = position;
    }

    /// <summary>Desenha uma esfera em wireframe de debug.</summary>
    public static void DrawWireSphere(Vector3 position, float radius, Color color, float duration = 0f, int segments = 16)
    {
        if (_instance == null) return;

        var mesh = new ImmediateMesh();
        mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);

        // Desenha 3 círculos ortogonais (XY, XZ, YZ)
        DrawCircleOnMesh(mesh, position, radius, Vector3.Up,      Vector3.Right,   color, segments);
        DrawCircleOnMesh(mesh, position, radius, Vector3.Right,   Vector3.Forward, color, segments);
        DrawCircleOnMesh(mesh, position, radius, Vector3.Forward, Vector3.Up,      color, segments);

        mesh.SurfaceEnd();

        _instance.CreateShapeInstance(mesh, duration, color, vertexColor: true);
    }

    /// <summary>Desenha uma caixa sólida de debug.</summary>
    public static void DrawBox(Vector3 position, Vector3 size, Color color, float duration = 0f)
    {
        if (_instance == null) return;

        var mesh = new BoxMesh { Size = size };
        var node = _instance.CreateShapeInstance(mesh, duration, color);
        node.GlobalPosition = position;
    }

    /// <summary>Desenha uma caixa em wireframe de debug.</summary>
    public static void DrawWireBox(Vector3 position, Vector3 size, Color color, float duration = 0f)
    {
        if (_instance == null) return;

        Vector3 half = size * 0.5f;

        // 8 vértices do cubo (local space)
        var v = new Vector3[]
        {
            position + new Vector3(-half.X, -half.Y, -half.Z),
            position + new Vector3( half.X, -half.Y, -half.Z),
            position + new Vector3( half.X, -half.Y,  half.Z),
            position + new Vector3(-half.X, -half.Y,  half.Z),
            position + new Vector3(-half.X,  half.Y, -half.Z),
            position + new Vector3( half.X,  half.Y, -half.Z),
            position + new Vector3( half.X,  half.Y,  half.Z),
            position + new Vector3(-half.X,  half.Y,  half.Z),
        };

        var mesh = new ImmediateMesh();
        mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);

        // Arestas inferiores
        AddLine(mesh, v[0], v[1], color); AddLine(mesh, v[1], v[2], color);
        AddLine(mesh, v[2], v[3], color); AddLine(mesh, v[3], v[0], color);
        // Arestas superiores
        AddLine(mesh, v[4], v[5], color); AddLine(mesh, v[5], v[6], color);
        AddLine(mesh, v[6], v[7], color); AddLine(mesh, v[7], v[4], color);
        // Arestas verticais
        AddLine(mesh, v[0], v[4], color); AddLine(mesh, v[1], v[5], color);
        AddLine(mesh, v[2], v[6], color); AddLine(mesh, v[3], v[7], color);

        mesh.SurfaceEnd();

        _instance.CreateShapeInstance(mesh, duration, color, vertexColor: true);
    }

    // -------------------------------------------------------------------------
    // AUXILIARES INTERNOS
    // -------------------------------------------------------------------------

    private static void AddLine(ImmediateMesh mesh, Vector3 a, Vector3 b, Color color)
    {
        mesh.SurfaceSetColor(color);
        mesh.SurfaceAddVertex(a);
        mesh.SurfaceSetColor(color);
        mesh.SurfaceAddVertex(b);
    }

    private static void DrawCircleOnMesh(
        ImmediateMesh mesh, Vector3 center, float radius,
        Vector3 axisA, Vector3 axisB, Color color, int segments)
    {
        for (int i = 0; i < segments; i++)
        {
            float a0 = Mathf.Tau * i       / segments;
            float a1 = Mathf.Tau * (i + 1) / segments;

            Vector3 p0 = center + (axisA * Mathf.Cos(a0) + axisB * Mathf.Sin(a0)) * radius;
            Vector3 p1 = center + (axisA * Mathf.Cos(a1) + axisB * Mathf.Sin(a1)) * radius;

            mesh.SurfaceSetColor(color); mesh.SurfaceAddVertex(p0);
            mesh.SurfaceSetColor(color); mesh.SurfaceAddVertex(p1);
        }
    }

    /// <summary>
    /// Cria um material único por shape.
    /// — vertexColor = true  → usa cor dos vértices (ImmediateMesh)
    /// — vertexColor = false → usa AlbedoColor (SphereMesh, BoxMesh, etc.)
    /// </summary>
    private MeshInstance3D CreateShapeInstance(Mesh mesh, float duration, Color color, bool vertexColor = false)
    {
        // CORREÇÃO: material exclusivo por instância — sem isso todas as shapes
        // compartilhariam a mesma cor (última cor aplicada ganharia).
        var material = new StandardMaterial3D
        {
            ShadingMode           = StandardMaterial3D.ShadingModeEnum.Unshaded,
            VertexColorUseAsAlbedo = vertexColor,
            AlbedoColor           = vertexColor ? Colors.White : color,
            // Transparência: permite cores com alpha < 1 (ex: new Color(1,0,0,0.4f))
            Transparency          = color.A < 1f
                                    ? StandardMaterial3D.TransparencyEnum.Alpha
                                    : StandardMaterial3D.TransparencyEnum.Disabled,
            // NoDepthTest = true → shapes aparecem sobre a geometria (útil para raycast debug)
            NoDepthTest           = false,
            CullMode              = BaseMaterial3D.CullModeEnum.Disabled,
        };

        var meshInstance = new MeshInstance3D
        {
            Mesh             = mesh,
            MaterialOverride = material,
            // CastShadow desligado: shapes de debug não precisam projetar sombra
            CastShadow       = GeometryInstance3D.ShadowCastingSetting.Off,
        };

        GetTree().Root.AddChild(meshInstance);

        // CORREÇÃO: GetProcessDeltaTime() retorna double → cast para float
        float lifetime = duration <= 0.001f ? (float)GetProcessDeltaTime() : duration;
        _activeShapes.Add(new DebugShape { MeshInstance = meshInstance, RemainingTime = lifetime });

        return meshInstance;
    }
}