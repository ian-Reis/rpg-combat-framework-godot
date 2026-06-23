@tool
extends BTAction
## Dispara o ataque do inimigo via PawnAI3D.Attack() (anima + hitbox no inimigo concreto).
## SUCCESS após disparar; FAILURE se o agente não expõe Attack().

func _generate_name() -> String:
	return "Attack"

func _tick(_delta: float) -> Status:
	if agent.has_method("Attack"):
		agent.Attack()
		return SUCCESS
	push_warning("[Attack] agente '%s' não tem método Attack()" % agent.name)
	return FAILURE
