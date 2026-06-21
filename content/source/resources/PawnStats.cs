using Godot;
using System;

namespace RPGFramework.Resources;

[GlobalClass]
public partial class PawnStats : Resource
{
    [Export]
    public float Speed = 5.0f;
}