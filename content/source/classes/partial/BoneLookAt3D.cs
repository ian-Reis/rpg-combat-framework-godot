using Godot;
using RPGFramework.Entities;

namespace RPGFramework.Core;

[GlobalClass]
public partial class BoneLookAt3D : LookAtModifier3D
{
    [ExportGroup("References")]
    [Export] public Pawn3D Pawn { get; set; }

    [ExportGroup("")]
    [Export] public Godot.Collections.Array<StringName> TargetGroup { get; set; }
    [Export] public float MaxDistance { get; set; } = 50f;
    [Export] public float MinDistance { get; set; } = 5f;
    [Export] public float InfluenceBlend { get; set; } = 2f;

    [ExportGroup("Mouse Aim")]
    [Export] public SpringArm3D SpringArm { get; set; }
    [Export] public uint AimCollisionMask { get; set; } = 1;
    [Export] public float MouseAimDistance { get; set; } = 30f;
    [Export] public bool UseMouseAim { get; set; } = false;

    private Node3D _mouseAimTarget;
    private float _influenceBlend = 0f;

    public override void _Ready()
    {
        _mouseAimTarget = new Node3D();
        AddChild(_mouseAimTarget);

        // Garante posição inicial válida antes do primeiro _Process
        if (SpringArm is not null)
            _mouseAimTarget.GlobalPosition = SpringArm.GlobalPosition + (-SpringArm.GlobalTransform.Basis.Z * MouseAimDistance);
    }

    // Raycast físico SÓ aqui, nunca em _Process
    public override void _PhysicsProcess(double delta)
    {
        if (UseMouseAim && SpringArm is not null && Pawn is not null)
            UpdateMouseAim();
    }

    public override void _Process(double delta)
    {
        float dt = Setup(delta);

        if (!Active) return;

        HandleAim(dt);
        AutoAim();
    }

    private float Setup(double delta)
    {
        if (Pawn is null)
        {
            Active = false;
            return 0f;
        }

        if (!UseMouseAim && TargetGroup.Count <= 0)
        {
            Active = false;
            return 0f;
        }

        Active = true;
        return (float)delta;
    }

    protected virtual void HandleAim(float dt)
    {
        float target = Pawn.State.IsAimed ? 1f : 0f;
        _influenceBlend = Mathf.Lerp(_influenceBlend, target, 1f - Mathf.Exp(-InfluenceBlend * dt));
        Influence = _influenceBlend;
    }

    private void AutoAim()
    {
        if (UseMouseAim)
        {
            if (SpringArm is null)
            {
                GD.PrintErr("BoneLookAt3D: UseMouseAim está ativo mas SpringArm não foi atribuído.");
                TargetNode = null;
                return;
            }

            // Só lê a posição já calculada em _PhysicsProcess
            TargetNode = _mouseAimTarget.GetPath();
            return;
        }

        var nodes = GetNodes();
        Node3D closestNode = null;
        float closestDistance = MaxDistance;

        foreach (Node3D node in nodes)
        {
            if (!IsInstanceValid(node))
                continue;

            float distance = node.GlobalPosition.DistanceTo(Pawn.GlobalPosition);

            if (distance < closestDistance && distance > MinDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }

        TargetNode = IsInstanceValid(closestNode) ? closestNode.GetPath() : null;
    }

    private void UpdateMouseAim()
    {
        Basis springBasis = SpringArm.GlobalTransform.Basis;
        Vector3 rayOrigin = SpringArm.GlobalPosition;
        Vector3 rayDir = -springBasis.Z;

        Vector3 rayEnd = rayOrigin + rayDir * MouseAimDistance;

        var spaceState = Pawn.GetWorld3D().DirectSpaceState;
        var query = PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd, AimCollisionMask);
        var result = spaceState.IntersectRay(query);

        if (result.Count > 0)
            _mouseAimTarget.GlobalPosition = (Vector3)result["position"];
        else
            _mouseAimTarget.GlobalPosition = rayEnd;
    }

    private Godot.Collections.Array<Node> GetNodes()
    {
        var allNodes = new Godot.Collections.Array<Node>();

        foreach (StringName group in TargetGroup)
        {
            if (!GetTree().HasGroup(group))
                continue;

            Godot.Collections.Array<Node> nodesInGroup = GetTree().GetNodesInGroup(group);

            foreach (Node3D node in nodesInGroup)
            {
                if (!allNodes.Contains(node))
                    allNodes.Add(node);
            }
        }

        return allNodes;
    }
}