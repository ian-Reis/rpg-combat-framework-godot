using System.Diagnostics;
using Godot;
using Data;

namespace Components;

// Holds design-time data (EnemyData or NPCData) for an AI-controlled entity.
// Place as child of SystemLogicComponents. BTPlayer lives here with agent_node = "../..".
[GlobalClass]
public partial class BrainComponent : Node
{
    [Export] public EnemyData EnemyData;
    [Export] public NPCData   NPCData;

    public float  AttackRange       => EnemyData?.AttackRange       ?? 2f;
    public float  FleeHealthPercent => EnemyData?.FleeHealthPercent ?? 0f;
    public string Faction           => EnemyData?.Faction ?? NPCData?.Faction ?? "neutral";

    public override void _Ready()
    {
        Debug.Assert(GetParentOrNull<SystemLogicComponents>() != null,
            "BrainComponent must be a child of SystemLogicComponents");
    }
}
