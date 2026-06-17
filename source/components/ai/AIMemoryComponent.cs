using Godot;

namespace Components;

// Remembers the last known position of AIDetectionComponent's target.
// Useful for search behaviors when the target is lost.
[GlobalClass]
public partial class AIMemoryComponent : Node
{
    [Export] public float MemoryDuration = 5f;  // seconds before memory fades

    public Vector3 LastKnownPosition { get; private set; }
    public float   TimeSinceSeen     { get; private set; } = float.MaxValue;
    public bool    HasMemory         => TimeSinceSeen < MemoryDuration;

    private AIDetectionComponent _detection;

    public override void _Ready()
    {
        var owner = GetParentOrNull<SystemLogicComponents>();
        _detection = owner?.GetComponent<AIDetectionComponent>();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_detection?.CurrentTarget != null && IsInstanceValid(_detection.CurrentTarget))
        {
            LastKnownPosition = _detection.CurrentTarget.GlobalPosition;
            TimeSinceSeen     = 0f;
        }
        else
        {
            TimeSinceSeen += (float)delta;
        }
    }
}
