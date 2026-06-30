using System.ComponentModel;
using System.IO;
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

    private float _influenceBlend = 0f;
    public override void _Process(double delta)
    {
        float dt = Setup(delta); 

        if (!Active) return; 

        HandleAim(dt);
        AutoAim();
    }

    private float Setup(double delta)
    {
        if (Pawn is null || TargetGroup.Count <= 0)
        {
            Active = false;
            return 0f;
        }
        
        Active = true;
        return ((float)delta);
    }

    protected virtual void HandleAim(float dt)
    {
        // Active = Pawn.State.IsAimed ? true : false;
        float target = Pawn.State.IsAimed ? 1f : 0f;
        _influenceBlend = Mathf.Lerp(_influenceBlend, target, 1f - Mathf.Exp(-InfluenceBlend * dt));
        Influence = _influenceBlend;
    }

    private void AutoAim()
    {
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

        if (IsInstanceValid(closestNode))
            TargetNode = closestNode.GetPath();
        else
            TargetNode = null;
    }

    private Godot.Collections.Array<Node> GetNodes()
    {
        // Cria uma nova array para acumular todos os nós encontrados
        var allNodes = new Godot.Collections.Array<Node>();

        foreach (StringName group in TargetGroup)
        {
            // Se o grupo não existir ou estiver vazio, pula para o próximo
            if (!GetTree().HasGroup(group))
                continue;
                
            Godot.Collections.Array<Node> nodesInGroup = GetTree().GetNodesInGroup(group);
            
            // Adiciona os nós deste grupo à nossa lista principal
            foreach (Node3D node in nodesInGroup)
            {
                // Evita duplicar o mesmo nó caso ele esteja em mais de um grupo
                if (!allNodes.Contains(node))
                {
                    allNodes.Add(node);
                }
            }
        }

        // Retorna a lista (estará vazia se nenhum nó for encontrado, evitando erros de 'null')
        return allNodes;
    }


 }