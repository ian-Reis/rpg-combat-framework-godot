@tool
extends BTAction
## Pede ao PawnAI3D (agente) que se mova até o alvo do blackboard.
## RUNNING enquanto longe; SUCCESS ao entrar em arrival_distance.
## O movimento real (gravidade, velocidade do Stats, facing) é do próprio pawn.

@export var target_var: StringName = &"target"
@export var arrival_distance: float = 1.5

func _generate_name() -> String:
	return "MoveToTarget  %s  ≤ %.1f" % [LimboUtility.decorate_var(target_var), arrival_distance]

func _tick(_delta: float) -> Status:
	var target := blackboard.get_var(target_var) as Node3D
	if not is_instance_valid(target):
		return FAILURE

	var body := agent as Node3D
	var to_target: Vector3 = target.global_position - body.global_position
	to_target.y = 0.0

	if to_target.length() <= arrival_distance:
		if agent.has_method("StopMoving"):
			agent.StopMoving()
		return SUCCESS

	if agent.has_method("MoveTowards"):
		agent.MoveTowards(target)
	return RUNNING
