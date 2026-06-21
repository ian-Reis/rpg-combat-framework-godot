using Godot;

namespace Data;

[GlobalClass]
public partial class NPCData : Resource
{
    [ExportGroup("Identity")]
    [Export] public string    NPCName = "";
    [Export] public Texture2D Portrait;

    [ExportGroup("Stats")]
    [Export] public PawnStats Stats;

    [ExportGroup("AI")]
    // Assign a LimboAI BehaviorTree resource here.
    [Export] public Resource BehaviorTree;
    [Export] public string   Faction = "neutral";

    [ExportGroup("Dialogue")]
    // Assign a Sprouty dialogue resource here.
    [Export] public Resource DialogueTree;
    [Export] public string   DefaultDialogueEntry = "";
}
