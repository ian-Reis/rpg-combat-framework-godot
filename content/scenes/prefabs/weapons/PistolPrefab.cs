using Godot;
using System;

public partial class PistolPrefab : Node3D
{
    [ExportGroup("References")]
    [Export] public Marker3D Marker3D { get; set; }
    [Export] public Node3D Model { get; set; }
    
    [ExportGroup("")]
    [Export] public PackedScene Projectile { get; set; }
    [Export] public float Cooldown { get; set; } = 1f;

    private bool IsPressedFire() => Input.IsActionJustPressed("attack");
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (!IsInstanceValid(Marker3D) || !IsInstanceValid(Projectile)) return;
        
        if (IsPressedFire())
        {
            Projectile3D proj = (Projectile3D)Projectile.Instantiate();
            
            // 1. CORREÇÃO CRUCIAL: Adicionar a bala à árvore de nós do mundo antes de mover
            GetTree().CurrentScene.AddChild(proj);
            
            // 2. CORREÇÃO: Definir a posição DEPOIS de dar o AddChild
            proj.GlobalTransform = Marker3D.GlobalTransform;
            
            // 3. FÍSICA: Na Godot, a frente do objeto é o Z NEGATIVO (-Basis.Z)
            proj.Setup(-Marker3D.GlobalTransform.Basis.Z);
        }
    }
}
