using Godot;

namespace Components;

// Scans for targets on a timer and exposes CurrentTarget.
// BT tasks read CurrentTarget — no scene-tree scanning per tick.
[GlobalClass]
public partial class AIDetectionComponent : Node
{
    [ExportGroup("Detection")]
    [Export] public string TargetGroup     = "Player";
    [Export] public float  ScanInterval    = 0.3f;
    [Export] public float  DetectionRadius = 10f;  // 0 = unlimited

    public Node3D CurrentTarget { get; private set; }

    private float                 _timer;
    private SystemLogicComponents _owner;

    public override void _Ready()
    {
        _owner = GetParentOrNull<SystemLogicComponents>();
        Scan();
    }

    public override void _PhysicsProcess(double delta)
    {
        _timer -= (float)delta;
        if (_timer > 0f) return;
        _timer = ScanInterval;
        Scan();
    }

    private void Scan()
    {
        Node3D pawn = _owner?.Pawn;
        if (pawn == null) { CurrentTarget = null; return; }

        float maxDist = DetectionRadius > 0f ? DetectionRadius : float.MaxValue;

        if (CurrentTarget != null && IsInstanceValid(CurrentTarget))
        {
            if (pawn.GlobalPosition.DistanceTo(CurrentTarget.GlobalPosition) <= maxDist)
                return;
        }

        CurrentTarget = null;
        float best = float.MaxValue;

        foreach (Node node in GetTree().GetNodesInGroup(TargetGroup))
        {
            if (node is not Node3D t || !IsInstanceValid(node)) continue;
            float d = pawn.GlobalPosition.DistanceTo(t.GlobalPosition);
            if (d < best && d <= maxDist) { best = d; CurrentTarget = t; }
        }
    }
}
