class_name TriangleAngle2D
extends Node2D

const SELECTION_COLOR = Color("#61c77b", 0.5)
const HIGHLIGHT_COLOR = Color(Color.GOLD, 0.5)
const KNOWN_COLOR = Color.LIGHT_BLUE
const TARGET_COLOR = Color.HOT_PINK

const center: Vector2 = Vector2(0,0)
const segments: int = 100
const radius: float = 70

var start_angle: float
var end_angle: float
var selection_arc: StaticBody2D
var polygon: Polygon2D
var point: Point

var known: bool = false:
	set(value):
		known = value
		update()

var target: bool = false

var highlighted: bool = false:
	set(value):
		highlighted = value
		update()

var allow_selection: bool = true

signal angle_selected(angle: TriangleAngle2D)

func _init(start: float = 0, end: float = PI) -> void:
	start_angle = start
	end_angle = end
	
	selection_arc = StaticBody2D.new()
	polygon = Polygon2D.new()
	var collision_area = Area2D.new()
	var collision_polygon = CollisionPolygon2D.new()
	
	var points: PackedVector2Array = [center]
	for i in range(segments):
		var angle = lerp_angle_clockwise(start_angle, end_angle, i / float(segments - 1))
		points.append(center + Vector2.from_angle(angle) * radius)
	
	polygon.polygon = points
	collision_polygon.polygon = points
	
	polygon.color = SELECTION_COLOR
	polygon.z_index = -20
	
	selection_arc.add_child(polygon)
	collision_area.add_child(collision_polygon)
	selection_arc.add_child(collision_area)
	add_child(selection_arc)
	
	collision_area.input_event.connect(_is_polygon_selected)
	

func _draw():
	const width: int = 3
	const radius: float = 40
	const center: Vector2 = Vector2(0, 0)

	if not known and not target:
		return
		
	var start_angle: float = start_angle
	var end_angle: float = end_angle
	
	var color
	if known:
		color = KNOWN_COLOR.darkened(0.1)
	elif target:
		color = TARGET_COLOR.darkened(0.1)
	
	if start_angle > end_angle:
		end_angle += TAU
		
	var angle_difference: float = end_angle - start_angle
	if is_equal_approx(angle_difference, 0.5 * PI):
		var start_point: Vector2 = Vector2.from_angle(start_angle) * radius
		var end_point: Vector2 = Vector2.from_angle(end_angle) * radius
		var diagonal_point: Vector2 = start_point + end_point
		draw_polyline([start_point, diagonal_point, end_point], color, width, true)
	else:
		const segments: int = 100
		draw_arc(center, radius, start_angle, end_angle, segments, color, width, true)


func _is_polygon_selected(viewport: Node, event: InputEvent, shape_idx: int):
	if not event is InputEventMouseButton:
		return
	
	if event.is_pressed() and allow_selection:
		allow_selection = false
		angle_selected.emit(self)
		print("polygon time!")


func lerp_angle_clockwise(start: float, end: float, t: float) -> float:
	if start < end:
		var angular_distance: float = -end + start
		return end + t * angular_distance
	else:
		var angular_distance: float = end + TAU - start
		return start + t * angular_distance


func hide_unused() -> void:
	if not highlighted:
		selection_arc.hide()

func show_selection() -> void:
	allow_selection = true
	selection_arc.show()

func update() -> void:
	if highlighted:
		polygon.color = HIGHLIGHT_COLOR
		return
		
	polygon.color = SELECTION_COLOR
	queue_redraw()


func angle() -> float:
	if start_angle > end_angle:
		return end_angle - start_angle + TAU
	
	return end_angle - start_angle


func get_points() -> Array[Point]:
	var r_points: Array[Point]
	for points in point.angles.keys():
		if point.angles[points] == self:
			r_points.assign(points)
			return r_points
	
	return r_points
