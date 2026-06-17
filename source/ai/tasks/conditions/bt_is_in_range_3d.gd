## Succeeds if the pawn is within [range] units of the blackboard target.
## Enable [use_attack_range] to read the distance from BrainComponent.attack_range instead.
@tool
extends BTCondition

@export var target_key: StringName = &"target"
@export var range: float = 5.0
@export var use_attack_range: bool = false

func _generate_name() -> String:
	var r = "attack_range" if use_attack_range else str(range)
	return "IsInRange3D [%s] <= %s" % [target_key, r]

func _tick(_delta: float) -> int:
	var target = blackboard.get_var(target_key, null)
	if not target is Node3D or not is_instance_valid(target):
		return FAILURE

	var pawn: Node3D = agent.Pawn
	if pawn == null:
		return FAILURE

	var check_range: float = range
	if use_attack_range:
		var brain = agent.BrainComponent
		if brain != null:
			check_range = brain.AttackRange

	var dist: float = pawn.global_position.distance_to((target as Node3D).global_position)
	return SUCCESS if dist <= check_range else FAILURE
