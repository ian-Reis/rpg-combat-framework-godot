## Succeeds if the entity's normalized health is below [threshold].
## Useful for triggering flee or defensive behaviors.
@tool
extends BTCondition

## Normalized threshold (0..1). 0.3 = flee below 30% HP.
@export_range(0.0, 1.0) var threshold: float = 0.3

func _generate_name() -> String:
	return "IsHealthBelow [%.0f%%]" % (threshold * 100.0)

func _tick(_delta: float) -> int:
	var health = agent.HealthComponent
	if health == null:
		return FAILURE
	return SUCCESS if health.NormalizedHealth <= threshold else FAILURE
