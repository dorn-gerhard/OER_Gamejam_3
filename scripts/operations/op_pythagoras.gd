extends OperationButton


func handles_selection(
	selected_lines: Array[TriangleLine2D],
	selected_angles: Array[TriangleAngle2D]
) -> bool:
	return (
		selected_angles.is_empty() and 
		selected_lines.size() == 2 and
		selected_lines.all(func(line: TriangleLine2D) -> bool: return line.known) and
		selected_lines[0].shares_point_with(selected_lines[1])
	)


func execute() -> bool:
	var line_a := level_manager.selected_lines[0]
	var line_b := level_manager.selected_lines[1]
	
	var points: Array[Point] = line_a.get_shared_points_with(line_b)
	if points.is_empty():
		return false
	
	if not Triangle.has_right_angle(points[0],points[1],points[2]):
		level_manager.clear_selection()
		move_made.emit()
		return false
	
	var line_c: TriangleLine2D = level_manager.lines[[points[0], points[2]]]
	line_c.known = true

	level_manager.clear_selection()
	move_made.emit()
	return true
