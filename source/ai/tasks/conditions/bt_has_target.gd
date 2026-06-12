## Succeeds if the blackboard target exists and is still a valid instance.
@tool
extends BTCondition

@export var target_key: StringName = &"target"

func _generate_name() -> String:
	return "HasTarget [%s]" % target_key

func _tick(_delta: float) -> int:
	var target = blackboard.get_var(target_key, null)
	if target == null or not is_instance_valid(target):
		return FAILURE
	return SUCCESS
