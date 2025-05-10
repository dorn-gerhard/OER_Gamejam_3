extends OperationButton



func handles_selection(
	selected_lines: Array[TriangleLine2D],
	selected_angles: Array[TriangleAngle2D]
) -> bool:
	if selected_angles.size() != 3:
		return false
	if not angles_in_same_triangle(selected_angles):
		return false
	var known_count := 0
	for a in selected_angles:
		if a.known:
			known_count += 1
			
	if known_count != 2:
		return false
	return true
	
func angles_in_same_triangle(angles: Array[TriangleAngle2D]) -> bool:
	var base_points := angles[0].get_points() 
	for i in [1, 2]:
		var pts := angles[i].get_points()
		if pts.size() != base_points.size():
			return false
		for p in pts:
			if not p in base_points:
				return false
	return true
	

func execute() -> bool:
	var angles = level_manager.selected_angles
	if angles.size() != 3:
		return false
	if not angles_in_same_triangle(angles):
		return false

	var sum_angles := 0.0
	var known_count := 0

	for a in angles:
		sum_angles += a.angle()
		
		if a.known:
			known_count += 1
	print(sum_angles)
	if known_count == 2 and is_equal_approx(sum_angles, PI):
		for a in angles:
			if not a.known:
				a.known = true
		return true

	return false
