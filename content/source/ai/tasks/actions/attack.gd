@tool
extends BTAction
## Dispara UM ataque e aguarda ele terminar (não spamma todo tick).
## RUNNING enquanto o golpe toca; SUCCESS ao terminar.

const START_TIMEOUT := 0.3  # se o ataque não engatar nesse tempo, libera o BT

var _started: bool = false
var _elapsed: float = 0.0

func _generate_name() -> String:
	return "Attack"

func _enter() -> void:
	_started = false
	_elapsed = 0.0
	if agent.has_method("Attack"):
		agent.Attack()
	else:
		push_warning("[Attack] agente '%s' não tem método Attack()" % agent.name)

func _tick(delta: float) -> Status:
	var attacking: bool = agent.has_method("IsAttacking") and agent.IsAttacking()

	if attacking:
		_started = true
		return RUNNING

	# Ainda não começou a atacar neste frame → espera o início (com timeout de segurança).
	if not _started:
		_elapsed += delta
		return SUCCESS if _elapsed >= START_TIMEOUT else RUNNING

	# Começou e já terminou.
	return SUCCESS
