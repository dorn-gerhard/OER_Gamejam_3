class_name OperationButton
extends Button

signal move_made
signal move_succeeded
signal move_failed

var level_manager: LevelManager


func _ready() -> void:
	pressed.connect(_on_pressed)


func _on_selection_changed() -> void:
	visible = handles_selection(level_manager.selected_lines, level_manager.selected_angles)


func handles_selection(
	selected_lines: Array[TriangleLine2D],
	selected_angles: Array[TriangleAngle2D]
) -> bool:
	return false


func _on_pressed() -> void:
	if execute():
		move_succeeded.emit()
	else:
		move_failed.emit()


func execute() -> bool:
	return true
