using Godot;

namespace Data;

[GlobalClass]
public partial class EnemyData : Resource
{
    [ExportGroup("Identity")]
    [Export] public string    EnemyName = "";
    [Export] public Texture2D Icon;

    [ExportGroup("Stats")]
    [Export] public PawnStats Stats;

    [ExportGroup("AI")]
    // Assign a LimboAI BehaviorTree resource here.
    [Export] public Resource BehaviorTree;
    [Export] public float    DetectionRadius    = 10f;
    [Export] public float    AttackRange        = 2f;
    [Export] public float    FleeHealthPercent  = 0f;   // flee when below this (0 = never)
    [Export] public string   Faction            = "enemy";

    [ExportGroup("Combat")]
    [Export] public AttackData[] AttackPatterns = [];

    [ExportGroup("Loot")]
    [Export] public LootTable LootTable;
}
