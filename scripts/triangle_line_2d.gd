class_name TriangleLine2D
extends Line2D

const UNKNOWN_COLOR = Color.BLACK
const KNOWN_COLOR = Color.LIGHT_BLUE
const TARGET_COLOR = Color.HOT_PINK
const HIGHLIGHT_COLOR = Color.GOLD

var known: bool = false:
	set(value):
		known = value
		update()

var target: bool = false:
	set(value):
		target = value
		update()

var highlighted: bool = false:
	set(value):
		highlighted = value
		update()

var parallel: bool = false

var triangle_points: Array[Point]
var background_line: Line2D
var parallel_markers: Array[Line2D]

func _init(p_line: Line, point_a: Point, point_b: Point) -> void:
	add_triangle_point(point_a)
	add_triangle_point(point_b)
	
	end_cap_mode = Line2D.LINE_CAP_ROUND
	begin_cap_mode = Line2D.LINE_CAP_ROUND
	width = 8
	antialiased = true

	known = p_line.known
	target = p_line.target
	parallel = p_line.parallel
	
	if parallel:
		generate_parallel_markers()
	generate_background_line()
	
	update()
	

func add_triangle_point(point: Point) -> void:
	add_point(point.position)
	triangle_points.append(point)

func update() -> void:
	if highlighted:
		update_color(HIGHLIGHT_COLOR)
		return
		
	if known:
		update_color(KNOWN_COLOR)
		return
	
	if target:
		update_color(TARGET_COLOR)
		return
	
	update_color(UNKNOWN_COLOR)
	

func update_color(color: Color) -> void:
	default_color = color
	for marker in parallel_markers:
		marker.default_color = color

func shares_point_with(other: TriangleLine2D) -> bool:
	var found: bool = false
	for point in triangle_points:
		if point in other.triangle_points:
			found = true
			break
	
	return found
	
	
func get_shared_points_with(other: TriangleLine2D) -> Array[Point]:
	if not shares_point_with(other):
		return []
	
	var left_point: Point
	var shared_point: Point
	var right_point: Point
	
	for point in triangle_points:
		if point in other.triangle_points:
			shared_point = point
		else:
			left_point = point
	
	for point in other.triangle_points:
		if point != shared_point:
			right_point = point
	
	return [left_point, shared_point, right_point]


func length() -> float:
	return (triangle_points[0].position - triangle_points[1].position).length()


func generate_background_line() -> void:
	background_line = Line2D.new()
	
	background_line.add_point(triangle_points[0].position)
	background_line.add_point(triangle_points[1].position)
	
	background_line.show_behind_parent = true
	background_line.z_index = -10
	background_line.width = width + 8.0
	background_line.default_color = Color.WHITE
	background_line.end_cap_mode = Line2D.LINE_CAP_ROUND
	background_line.begin_cap_mode = Line2D.LINE_CAP_ROUND
	background_line.antialiased = true
	add_child(background_line)

func generate_parallel_markers() -> void:
	var point_a: Vector2 = triangle_points[0].position
	var point_b: Vector2 = triangle_points[1].position
	
	var mid: Vector2 = (point_a + point_b) * 0.5
	var direction: Vector2 = (point_b - point_a).normalized()
	var ortho: Vector2 = direction.orthogonal()
	
	var marker_a = Line2D.new()
	var marker_b = Line2D.new()
	marker_a.add_point(mid - direction * 5 - ortho * 8)
	marker_a.add_point(mid - direction * 2 + ortho * 8)
	marker_b.add_point(mid + direction * 2 - ortho * 8)
	marker_b.add_point(mid + direction * 5 + ortho * 8)
	
	marker_a.width = 3
	marker_b.width = 3
	
	parallel_markers = [marker_a, marker_b]
	
	add_child(marker_a)
	add_child(marker_b)
	
