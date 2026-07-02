using Godot;
using RPGFramework.Entities;
using System;

[GlobalClass]
public partial class CustomShapeCast3D : ShapeCast3D
{
    [ExportGroup("References")]
    [Export] public Pawn3D Pawn { get; set; }

    [ExportGroup("")]
    [Export] public StringName GroupTarget { get; set; }

    // Variável de controle para registrar se o player já está no modo de corte
    private bool _isNearTree = false;
    private Node3D _currentTree = null;

    public override void _Process(double delta)
    {
        if (!Enabled) 
        {
            return;
        }

        // 1. Procura se há alguma árvore válida no frame atual
        Node3D detectedTree = CheckForValidTree();

        if (detectedTree != null)
        {
            _currentTree = detectedTree;

            // DISPARO ÚNICO (Entrou no alcance agora)
            if (!_isNearTree)
            {
                _isNearTree = true;
                Pawn.State.CanAttack = false; // Bloqueia ataque normal apenas UMA vez
                GD.Print("Entrou no alcance da árvore: Estado alterado (One-shot)");
            }

            // Lógica de ação (Input)
            if (Input.IsActionPressed("attack"))
            {
                Pawn.State.IsCutting = true;
                // _currentTree.QueueFree();
            }
        }
    }

    /// <summary>
    /// Varre as colisões do ShapeCast para encontrar uma árvore válida.
    /// </summary>
    private Node3D CheckForValidTree()
    {
        if (!IsColliding()) return null;

        int collisionCount = GetCollisionCount();
        for (int i = 0; i < collisionCount; i++)
        {
            GodotObject obj = GetCollider(i);
            if (!IsInstanceValid(obj)) continue;

            if (obj is Node3D tree && tree.IsInGroup(GroupTarget))
            {
                return tree; // Retorna a primeira árvore válida encontrada
            }
        }
        return null;
    }
}
