class_name Angle
extends Resource

# points in counterclockwise order with the angle at point b
@export var point_a: NodePath
@export var point_b: NodePath
@export var point_c: NodePath

@export var known := false
@export var target := false


func _to_string() -> String:
	return("/%s%s%s" % [point_a, point_b, point_c])
