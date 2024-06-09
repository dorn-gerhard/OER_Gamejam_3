extends Node2D

signal level_won

const MAX_LEVEL_NUM = 6

@onready var level_manager: LevelManager = $LevelManager
@onready var moves_label: Label = $MarginContainer/Label
@onready var actions: Container = $BottomUI/Actions
@onready var anim: AnimationPlayer = $AnimationPlayer

var moves: int = 0:
	set(value):
		moves = value
		moves_label.text = "Moves: %s" % moves


func _ready() -> void:
	for action_button in actions.get_children():
		action_button.level_manager = level_manager
		level_manager.selection_changed.connect(action_button._on_selection_changed)
		action_button.move_made.connect(_on_move_made)
		action_button.visible = false
		action_button.move_succeeded.connect(_on_move_succeeded)
		action_button.move_failed.connect(_on_move_failed)
		
	level_won.connect(_on_level_won)
	
	$AudioPlayer.connect("finished", Callable(self,"_on_loop_sound").bind($AudioPlayer))
	$AudioPlayer.play()


func _on_loop_sound(player):
	$AudioPlayer.stream_paused = false
	$AudioPlayer.play()


func _on_selection_changed() -> void:
	print(level_manager.selected_angles)
	print(level_manager.selected_lines)


func _on_move_made() -> void:
	moves += 1
	var unknown_target_quantities := 0
	for line in level_manager.lines.values():
		if line.target and not line.known:
			unknown_target_quantities += 1
			
	for angle in level_manager.angles.values():
		print(angle.target, " + " , angle.known)
		if angle.target and not angle.known:
			unknown_target_quantities += 1
	
	if unknown_target_quantities == 0:
		level_won.emit()


func _on_level_won() -> void:
	if level_manager.current_level == MAX_LEVEL_NUM:
		anim.play(&"game_won")
		await anim.animation_finished
		get_tree().quit()
	else:
		anim.play(&"level_won")
		await anim.animation_finished
		level_manager.load_level(level_manager.current_level + 1)


func _on_move_succeeded() -> void:
	if not anim.is_playing():
		anim.play(&"nice")


func _on_move_failed() -> void:
	anim.play(&"too_bad")
