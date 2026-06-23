@tool
extends Skeleton3D

@onready var simulator: PhysicalBoneSimulator3D = get_node("PhysicalBoneSimulator3D")

func _ready():
	simulator.physical_bones_start_simulation()
