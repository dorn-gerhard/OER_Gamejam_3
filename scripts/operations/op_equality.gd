extends OperationButton


func handles_selection(
	selected_lines: Array[TriangleLine2D],
	selected_angles: Array[TriangleAngle2D]
) -> bool:
	return (
		(
			selected_angles.is_empty() and 
			selected_lines.size() == 2 and
			(selected_lines[0].known or selected_lines[1].known)
		) or (
			selected_lines.is_empty() and 
			selected_angles.size() == 2 and
			(selected_angles[0].known or selected_angles[1].known)
		)
	)

func execute() -> bool:
	if level_manager.selected_lines.size() == 2:
		var line_a := level_manager.selected_lines[0]
		var line_b := level_manager.selected_lines[1]
		
		if not is_equal_approx(line_a.length(), line_b.length()):
			level_manager.clear_selection()
			move_made.emit()
			return false
		
		line_a.known = true
		line_b.known = true
		
	if level_manager.selected_angles.size() == 2:
		var angle_a := level_manager.selected_angles[0]
		var angle_b := level_manager.selected_angles[1]
		
		if not is_equal_approx(angle_a.angle(), angle_b.angle()):
			level_manager.clear_selection()
			move_made.emit()
			return false
		
		angle_a.known = true
		angle_b.known = true

	level_manager.clear_selection()
	move_made.emit()
	return true
