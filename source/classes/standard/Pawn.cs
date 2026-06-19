using Data;
using Godot;
using System;

partial class Pawn : CharacterBody3D
{
    [Export] public PawnStats Stats { get; set; }
}