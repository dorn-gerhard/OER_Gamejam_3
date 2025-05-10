class_name LevelManager
extends Node2D

@export var start_level := 1

var level: Level
static var lines = {}
static var angles = {}
var selected_point: Point
var selected_lines: Array[TriangleLine2D]
var selected_angles: Array[TriangleAngle2D]
var current_level: int

signal selection_changed

static var angle_selection_mode_active := false

func _ready() -> void:
	selection_changed.connect(_on_selection_changed)
	current_level = start_level
	load_level(start_level)


func load_level(level_number: int = 1) -> void:
	# Reset
	for child in get_children():
		child.queue_free()
	lines.clear()
	angles.clear()
	
	# Load
	current_level = level_number
	var level_path = "res://scenes/level%s.tscn" % level_number
	if not ResourceLoader.exists(level_path):
		print("Good job!")
		return
	
	level = load(level_path).instantiate()
	add_child(level)
	
	for line: Line in level.lines:
		var point_a: Point = level.get_node(line.point_a) as Point
		var point_b: Point = level.get_node(line.point_b) as Point
		
		point_a.connected_points.push_back(point_b)
		point_b.connected_points.push_back(point_a)
		
		var line_2d = TriangleLine2D.new(line, point_a, point_b)
		
		lines[[point_a, point_b]] = line_2d
		lines[[point_b, point_a]] = line_2d
		
		add_child(line_2d)
		
	for angle: Angle in level.angles:
		var point_a: Point = level.get_node(angle.point_a) as Point
		var point_b: Point = level.get_node(angle.point_b) as Point
		var point_c: Point = level.get_node(angle.point_c) as Point
		
		angles[[point_a,point_b,point_c]] = angle
	
	for point: Point in level.get_children():
		point.compute_angles(angles)
		
		for key in angles.keys():
			if key[1] == point:
				angles[key] = point.get_angle(key)
				
		point.released.connect(_on_point_released)
		point.selected.connect(_on_point_selected)
		
		for triangle_angle in point.angles.values():
			triangle_angle.angle_selected.connect(_on_angle_selected)


func _on_point_released(point: Point) -> void:
	if not lines.has([selected_point, point]):
		clear_selection()
		return
	var line: TriangleLine2D = lines[[selected_point, point]]
	line.highlighted = true
	selected_lines.push_back(line)
	selection_changed.emit()


func _on_point_selected(point: Point) -> void:
	print("Good job!")
	Input.set_default_cursor_shape(Input.CURSOR_HELP)
	selected_point = point
	for connected_point in point.connected_points:
		print(connected_point)
		connected_point.highlight(true)


func _notification(what: int) -> void:
	match what:
		NOTIFICATION_DRAG_END:
			for point: Point in level.get_children():
				point.highlight.call_deferred(false)

func _on_selection_changed() -> void:
	# make angle selectable if there are two lines selected
	if len(selected_lines) == 2:
		var line_a := selected_lines[0]
		var line_b := selected_lines[1]
		
		var points: Array[Point] = line_a.get_shared_points_with(line_b)
		if not points.is_empty():
			points[1].show_angle(points)
			LevelManager.angle_selection_mode_active = true
	else:
		for point: Point in level.get_children():
			point.hide_all_angles()
		LevelManager.angle_selection_mode_active = false

func _on_angle_selected(angle: TriangleAngle2D):
	selected_angles.append(angle)
	angle.highlighted = true
	
	for point: Point in level.get_children():
		point.hide_all_angles()
	
	LevelManager.angle_selection_mode_active = false
	deselect_lines()
	print(selected_lines, selected_angles)
	selection_changed.emit()

func clear_selection() -> void:
	deselect_lines()
	deselect_angles()
	selection_changed.emit()


func deselect_lines() -> void:
	for line in selected_lines:
		line.highlighted = false
	selected_lines.clear()


func deselect_angles() -> void:
	for angle in selected_angles:
		angle.highlighted = false
	selected_angles.clear()
	
