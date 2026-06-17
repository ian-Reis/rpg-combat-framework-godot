## Succeeds if DetectionComponent has a valid CurrentTarget.
## Optionally stores the target in the blackboard for other tasks (e.g. IsInRange3D).
@tool
extends BTCondition

@export var target_key: StringName = &"target"

func _generate_name() -> String:
	return "HasTarget [%s]" % target_key

func _tick(_delta: float) -> int:
	var detection = agent.AIDetectionComponent
	if detection == null:
		return FAILURE

	var target = detection.CurrentTarget
	if target == null or not is_instance_valid(target):
		return FAILURE

	blackboard.set_var(target_key, target)
	return SUCCESS
