@tool
extends BTAction
## Acha o alvo mais próximo de uma facção (grupo) e guarda no blackboard.
## O grupo bate com Pawn3D.FactionGroup: "player", "enemy", "npc", "pet".
## SUCCESS se encontrou; FAILURE se não há ninguém dessa facção.

@export var target_group: StringName = &"player"
@export var target_var: StringName = &"target"

func _generate_name() -> String:
	return "FindTarget  faction: \"%s\"  → %s" % [target_group, LimboUtility.decorate_var(target_var)]

func _tick(_delta: float) -> Status:
	var found := agent.get_tree().get_first_node_in_group(target_group)
	if found == null:
		return FAILURE
	blackboard.set_var(target_var, found)
	return SUCCESS
