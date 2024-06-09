extends OperationButton


func handles_selection(
	selected_lines: Array[TriangleLine2D],
	selected_angles: Array[TriangleAngle2D]
) -> bool:
	return not (selected_lines.is_empty() and selected_angles.is_empty())


func _on_pressed() -> void:
	level_manager.clear_selection()
