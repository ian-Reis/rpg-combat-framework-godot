## Rotates the pawn's Y axis toward the blackboard target.
## Returns RUNNING until aligned within [angle_threshold], then SUCCESS.
## Set [instant] to snap immediately and always return SUCCESS.
@tool
extends BTAction

@export var target_key: StringName = &"target"
@export var turn_speed: float = 8.0
@export var angle_threshold: float = 0.08  # radians (~4.5 degrees)
@export var instant: bool = false

func _generate_name() -> String:
	return "FaceTarget3D [%s]" % target_key

func _tick(delta: float) -> int:
	var target = blackboard.get_var(target_key, null)
	if not target is Node3D or not is_instance_valid(target):
		return FAILURE

	var pawn: Node3D = agent.Pawn
	if pawn == null:
		return FAILURE

	var dir: Vector3 = (target as Node3D).global_position - pawn.global_position
	dir.y = 0.0
	if dir.is_zero_approx():
		return SUCCESS

	dir = dir.normalized()

	if instant:
		pawn.global_transform.basis = Basis.looking_at(dir, Vector3.UP)
		return SUCCESS

	var target_basis := Basis.looking_at(dir, Vector3.UP)
	pawn.global_transform.basis = pawn.global_transform.basis.slerp(
		target_basis, clamp(turn_speed * delta, 0.0, 1.0)
	)

	var angle: float = pawn.global_transform.basis.z.angle_to(-dir)
	return SUCCESS if angle < angle_threshold else RUNNING
