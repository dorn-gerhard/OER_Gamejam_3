class_name Line
extends Resource

@export var point_a: NodePath
@export var point_b: NodePath
@export var known := false
@export var target := false
@export var parallel := false

func _to_string() -> String:
	return("%s -> %s" % [point_a, point_b])
