using Godot;
using RPGFramework.Entitys;

namespace RPGFramework.Core;

[GlobalClass]
public partial class MeshAnimation : Node3D
{
    [Export] public DefaultControllerAnimation AnimationController { get; set; }

    public override void _Ready()
    {
        var pawn = GetParent<Pawn3D>();

        if (pawn == null)               { GD.PrintErr("[MeshAnimation] Pawn3D não encontrado no parent!"); return; }
        if (AnimationController == null) { GD.PrintErr("[MeshAnimation] AnimationController não atribuído!"); return; }

        AnimationController.Pawn  = pawn;
        AnimationController.Stats = pawn.Stats;
        AnimationController.Setup();
    }
}
