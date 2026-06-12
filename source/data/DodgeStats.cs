using Godot;

namespace Data;

[GlobalClass]
public partial class DodgeStats : Resource
{
    [Export] public float Cooldown        = 0.8f;
    [Export] public float Duration        = 0.3f;
    [Export] public float Speed           = 10f;
    [Export] public float IFramesDuration = 0.25f;
}
