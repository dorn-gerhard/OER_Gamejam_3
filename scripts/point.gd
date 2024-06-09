class_name Point
extends Control

signal selected(point: Point)
signal released(point: Point)

var connected_points: Array[Point] = []
var is_highlighted := false

var angles: Dictionary = {} # [Point, Point, Point] -> TriangleAngle2D

func _get_drag_data(at_position: Vector2) -> Variant:
	if LevelManager.angle_selection_mode_active:
		return null
	selected.emit(self)
	return self


func _can_drop_data(at_position: Vector2, data: Variant) -> bool:
	return get_viewport().gui_get_drag_data() != self


func _drop_data(at_position: Vector2, data: Variant) -> void:
	released.emit(self)

func highlight(do_highlight: bool = true) -> void:
	is_highlighted = do_highlight
	
	#var gradient := $Point.texture.gradient as Gradient
	#gradient.colors[0] = Color.YELLOW if do_highlight else Color.BLACK


func compute_angles(known_angles: Dictionary):
	connected_points.sort_custom(
		func (a: Point, b: Point) -> bool: 
			return position.angle_to_point(a.position) < \
				   position.angle_to_point(b.position)
	)
	
	for i in range(len(connected_points)):
		var prev_point: Point = connected_points[(i+len(connected_points)-1)%len(connected_points)]
		var next_point: Point = connected_points[i]
		
		var start_angle = position.angle_to_point(prev_point.position)
		var end_angle = position.angle_to_point(next_point.position)
		
		var angle: TriangleAngle2D = TriangleAngle2D.new(start_angle, end_angle)
		angle.point = self
		
		var key = [next_point, self, prev_point]
		if key in known_angles:
			angle.known = known_angles[key].known
			angle.target = known_angles[key].target
		
		angles[[prev_point, self, next_point]] = angle
		angle.hide_unused()
		add_child(angle)



func _to_string() -> String:
	return name


func has_angle_approx(p_angle) -> bool:
	for angle: TriangleAngle2D in angles.values():
		
		if is_equal_approx(p_angle, angle.start_angle) or \
				is_equal_approx(p_angle, angle.end_angle):
			return true
	
	return false


func show_angle(key: Array[Point]):
	var key_a = [key[0], key[1], key[2]]
	var key_b = [key[2], key[1], key[0]]
	if key_a in angles:
		angles[key_a].show_selection()
	if key_b in angles:
		angles[key_b].show_selection()


func hide_all_angles():
	for angle: TriangleAngle2D in angles.values():
		angle.hide_unused()

func get_angle(key) -> TriangleAngle2D:
	var inv_key = [key[2], key[1], key[0]]
	
	if key in angles and inv_key in angles:
		if angles[key].known or angles[key].target:
			return angles[key]
		else:
			return angles[inv_key]
	
	if key in angles:
		return angles[key]
	else:
		return angles[inv_key]
