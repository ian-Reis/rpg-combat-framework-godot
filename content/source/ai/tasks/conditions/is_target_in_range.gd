@tool
extends BTCondition
## SUCCESS se o alvo do blackboard está dentro de 'attack_range' (plano horizontal); senão FAILURE.

@export var target_var: StringName = &"target"
@export var attack_range: float = 2.0

func _generate_name() -> String:
	return "IsTargetInRange  %s  ≤ %.1f" % [LimboUtility.decorate_var(target_var), attack_range]

func _tick(_delta: float) -> Status:
	var target := blackboard.get_var(target_var) as Node3D
	if not is_instance_valid(target):
		return FAILURE

	var a: Vector3 = (agent as Node3D).global_position
	var b: Vector3 = target.global_position
	a.y = 0.0
	b.y = 0.0
	return SUCCESS if a.distance_to(b) <= attack_range else FAILURE
