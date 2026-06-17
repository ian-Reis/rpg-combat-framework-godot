## Finds the nearest node in [target_group] and stores it in the blackboard.
## Optionally restricted to BrainComponent.detection_radius.
@tool
extends BTAction

@export var target_key: StringName = &"target"
@export var target_group: String = "player"
## When true, only detects targets within BrainComponent.detection_radius.
@export var use_detection_radius: bool = true

func _generate_name() -> String:
	return "FindTarget3D [%s]" % target_group

func _tick(_delta: float) -> int:
	var pawn: Node3D = agent.Pawn
	if pawn == null:
		return FAILURE

	var max_dist: float = INF
	if use_detection_radius:
		var brain = agent.BrainComponent
		if brain != null:
			max_dist = brain.DetectionRadius

	var nearest = null
	var nearest_dist: float = INF

	for t in agent.get_tree().get_nodes_in_group(target_group):
		if not t is Node3D or not is_instance_valid(t):
			continue
		var d: float = pawn.global_position.distance_to((t as Node3D).global_position)
		if d < nearest_dist and d <= max_dist:
			nearest_dist = d
			nearest = t

	blackboard.set_var(target_key, nearest)
	return SUCCESS if nearest != null else FAILURE
