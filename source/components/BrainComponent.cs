using System.Diagnostics;
using Godot;
using Data;

namespace Components;

// Holds the data (EnemyData or NPCData) for an AI-controlled entity.
//
// Scene setup (SystemLogicComponents is a child of the root CharacterBody3D):
//
//   CharacterBody3D              ← scene root / Pawn
//   └── SystemLogicComponents
//       ├── BrainComponent       ← assign EnemyData or NPCData here
//       │   └── BTPlayer         ← agent_node = ".." (points to SystemLogicComponents)
//       └── other components
//
// GDScript tasks access the entity via:
//   agent                                → SystemLogicComponents
//   agent.pawn                           → CharacterBody3D (the root)
//   agent.logic_state_machine_component  → LogicStateMachineComponent
//   agent.health_component               → HealthComponent
//   agent.brain_component.enemy_data     → EnemyData
[GlobalClass]
public partial class BrainComponent : Node
{
    // Set one of these depending on entity type; leave the other null.
    [Export] public EnemyData EnemyData;
    [Export] public NPCData   NPCData;

    // Convenience properties read by GDScript tasks.
    public float  DetectionRadius   => EnemyData?.DetectionRadius ?? 10f;
    public float  AttackRange       => EnemyData?.AttackRange     ?? 2f;
    public float  FleeHealthPercent => EnemyData?.FleeHealthPercent ?? 0f;
    public string Faction           => EnemyData?.Faction ?? NPCData?.Faction ?? "neutral";

    private SystemLogicComponents _owner;

    public override void _Ready()
    {
        _owner = GetParentOrNull<SystemLogicComponents>();
        Debug.Assert(_owner != null, "BrainComponent must be a child of SystemLogicComponents");
    }
}
