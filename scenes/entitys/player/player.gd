extends CharacterBody3D


func _physics_process(delta):
	# ... seu código de movimento normal aqui ...
	
	# Envia a posição global do jogador direto para o Shader Global da Godot
	RenderingServer.global_shader_parameter_set("player_position", global_position)
