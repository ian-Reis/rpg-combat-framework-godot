## Tells the entity's LogicStateMachineComponent to change to [state_name].
## Always returns SUCCESS after requesting the transition.
@tool
extends BTAction

@export var state_name: String = ""

func _generate_name() -> String:
	return "SetLogicState [%s]" % state_name

func _tick(_delta: float) -> int:
	if state_name.is_empty():
		return FAILURE

	var lsm = agent.LogicStateMachineComponent
	if lsm == null:
		return FAILURE

	if lsm.CurrentStateName != state_name:
		lsm.ChangeState(state_name)
	return SUCCESS
