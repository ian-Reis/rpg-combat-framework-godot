using Godot;

namespace Components;

// Remembers the last known position of the detection target.
// Useful for search behaviors when the target is lost.
[GlobalClass]
public partial class AIMemoryComponent : Node
{
    [Export] public float MemoryDuration = 5f;

    public Vector3 LastKnownPosition { get; private set; }
    public float   TimeSinceSeen     { get; private set; } = float.MaxValue;
    public bool    HasMemory         => TimeSinceSeen < MemoryDuration;

    private Pawn _pawn;

    public override void _Ready()
    {
        _pawn = GetParent<Pawn>();
    }

    public override void _PhysicsProcess(double delta)
    {
        var detection = _pawn?.Detection;
        if (detection?.CurrentTarget != null && IsInstanceValid(detection.CurrentTarget))
        {
            LastKnownPosition = detection.CurrentTarget.GlobalPosition;
            TimeSinceSeen     = 0f;
        }
        else
        {
            TimeSinceSeen += (float)delta;
        }
    }
}
