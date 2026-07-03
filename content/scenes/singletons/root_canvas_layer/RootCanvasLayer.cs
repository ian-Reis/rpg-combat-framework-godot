using Godot;
using System;

public partial class RootCanvasLayer : CanvasLayer
{
    [Export] public Hud Hud { get; set; }

    public static RootCanvasLayer Self { get; private set; }

    public override void _Ready()
    {
        Self = this;
    }

}