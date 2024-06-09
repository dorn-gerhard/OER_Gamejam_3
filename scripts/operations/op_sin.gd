extends OperationButton


func handles_selection(
	selected_lines: Array[TriangleLine2D],
	selected_angles: Array[TriangleAngle2D]
) -> bool:
	for line in selected_lines:
		if not line.known: return false
	for angle in selected_angles:
		if not angle.known: return false
	return (selected_angles.size() == 1 and 
			selected_lines.size() == 1) or \
			(selected_lines.size() == 2 and
			selected_angles.size() == 0)


func execute() -> bool:
	if level_manager.selected_angles.size() == 1:
		var line := level_manager.selected_lines[0]
		var angle := level_manager.selected_angles[0]
		
		var tri := Triangle.from_angle_and_line(angle, line)
		if tri == null or is_equal_approx(abs(angle.start_angle - angle.end_angle), PI/2):
			level_manager.clear_selection()
			move_made.emit()
			return false
		
		var discovered_line: TriangleLine2D
		if line == tri.hypothenuse:
			if angle == tri.alpha:
				discovered_line = tri.leg_a
			elif angle == tri.beta:
				discovered_line = tri.leg_b
		else:
			discovered_line = tri.hypothenuse
		
		discovered_line.known = true
		level_manager.clear_selection()
		move_made.emit()
		return true
	else:
		var tri = Triangle.from_two_lines(level_manager.selected_lines)
		if tri == null:
			level_manager.clear_selection()
			move_made.emit()
			return false
		
		var hypothenuse: TriangleLine2D
		var discovered_angle: TriangleAngle2D
		for line in level_manager.selected_lines:
			if line == tri.hypothenuse:
				hypothenuse = line
			elif line == tri.leg_a:
				discovered_angle = tri.alpha
			elif line == tri.leg_b:
				discovered_angle = tri.beta
		
		if hypothenuse == null:
			level_manager.clear_selection()
			move_made.emit()
			return false
		
		discovered_angle.known = true
		level_manager.clear_selection()
		move_made.emit()
		return true
