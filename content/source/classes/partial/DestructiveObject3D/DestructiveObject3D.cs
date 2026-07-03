using Godot;
using RPGFramework.Helpers;

namespace RPGFramework.Core;

[GlobalClass]
public partial class DestructiveObject3D : RigidBody3D
{
    [ExportGroup("References")]
    [Export] public HurtBoxArea3D HurtBoxArea3D;
    [Export] public HealthComponent HealthComponent;
    [Export] public AnimationPlayer AnimationPlayer;

    private StringName _myGroup = "destructive";

    public override void _Ready()
    {
        RegisterGroup.Add(this, _myGroup);

        // Connect Signals
        HealthComponent.HealthChanged += Hit;
        HealthComponent.Died += Destroy;
    }

    public virtual void Hit(float current, float max)
    {
        GD.Print("          lasjkdafslfslfsdfajkl");
        // default example
        if (IsInstanceValid(AnimationPlayer))
        {
            if (AnimationPlayer.HasAnimation("hit")) 
                AnimationPlayer.Play("hit");
        }

    }

    public virtual void Destroy() => QueueFree();

}