using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
    [ExportGroup("References")]
    [Export] public AnimationTree AnimationTree;
    [Export] public Node3D Model;

    [Export] public StringName TargetGroup = "Player";
    [Export] public float Health = 100f;
    [Export] public float Speed = 5f;

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

}
