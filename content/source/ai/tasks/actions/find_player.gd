@tool
extends BTAction
## Procura o player (por grupo) e guarda a referência no blackboard.
## SUCCESS se encontrou, FAILURE se não há ninguém no grupo.

@export var group: StringName = &"player"
@export var target_var: StringName = &"target"

func _generate_name() -> String:
	return "FindPlayer  group: \"%s\"  → %s" % [group, LimboUtility.decorate_var(target_var)]

func _tick(_delta: float) -> Status:
	var found := agent.get_tree().get_first_node_in_group(group)
	if found == null:
		return FAILURE
	blackboard.set_var(target_var, found)
	return SUCCESS
