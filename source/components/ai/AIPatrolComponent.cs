using Godot;

namespace Components;

// Manages a sequence of waypoints for patrol behavior.
// BT tasks call NextWaypoint() and check IsAtWaypoint() each tick.
[GlobalClass]
public partial class AIPatrolComponent : Node
{
    [Export] public Node3D[] Waypoints = [];
    [Export] public bool     Loop      = true;

    public Node3D CurrentWaypoint => _index < Waypoints?.Length ? Waypoints[_index] : null;
    public bool   HasWaypoints    => Waypoints != null && Waypoints.Length > 0;
    public int    WaypointIndex   => _index;

    private int _index = 0;

    public void NextWaypoint()
    {
        if (!HasWaypoints) return;
        _index++;
        if (Loop)
            _index %= Waypoints.Length;
        else
            _index = Mathf.Min(_index, Waypoints.Length - 1);
    }

    public bool IsAtWaypoint(Node3D pawn, float threshold = 0.5f)
    {
        var wp = CurrentWaypoint;
        if (wp == null || pawn == null) return false;
        return pawn.GlobalPosition.DistanceTo(wp.GlobalPosition) <= threshold;
    }

    public void ResetPatrol() => _index = 0;
}
