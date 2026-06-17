## Moves the pawn toward the blackboard target each tick.
## Returns RUNNING while moving, SUCCESS when within stop_distance, FAILURE if no target.
## Sets CharacterBody3D.velocity (horizontal only) — LogicStateAIBody applies gravity and slides.
@tool
extends BTAction

@export var target_key: StringName = &"target"
## Override speed. Leave 0 to use CharacterStats.WalkSpeed.
@export var speed: float = 0.0
## Distance at which the task considers itself done.
@export var stop_distance: float = 1.5

func _generate_name() -> String:
	return "MoveTowardTarget [%s]" % target_key

func _tick(_delta: float) -> int:
	var target = blackboard.get_var(target_key, null)
	if not target is Node3D or not is_instance_valid(target):
		return FAILURE

	var pawn = agent.Pawn
	if not pawn is CharacterBody3D:
		return FAILURE

	var dir: Vector3 = target.global_position - pawn.global_position
	dir.y = 0.0

	if dir.length() <= stop_distance:
		pawn.velocity.x = 0.0
		pawn.velocity.z = 0.0
		return SUCCESS

	dir = dir.normalized()

	var move_speed: float = speed
	if move_speed <= 0.0:
		var stats = agent.Stats
		move_speed = stats.WalkSpeed if stats != null else 3.0

	pawn.velocity.x = dir.x * move_speed
	pawn.velocity.z = dir.z * move_speed
	return RUNNING
