using Godot;
using Data;

namespace Components;

// Holds design-time data (EnemyData or NPCData) for an AI-controlled entity.
[GlobalClass]
public partial class BrainComponent : Node
{
    [Export] public EnemyData EnemyData;
    [Export] public NPCData   NPCData;

    public float  AttackRange       => EnemyData?.AttackRange       ?? 2f;
    public float  FleeHealthPercent => EnemyData?.FleeHealthPercent ?? 0f;
    public string Faction           => EnemyData?.Faction ?? NPCData?.Faction ?? "neutral";
}
