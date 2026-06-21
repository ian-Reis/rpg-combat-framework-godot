using Godot;
using System;
using RPGFramework.Resources;

namespace RPGFramework.Entitys;

[GlobalClass]
public partial class Pawn : CharacterBody3D
{
    [Export]
    public PawnStats Stats;

    public Vector2 Motion {get; private set;}

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

}