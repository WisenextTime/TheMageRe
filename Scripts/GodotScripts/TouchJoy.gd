extends TextureRect

@export var diedZone := 0.5
@export var maxLength := 90.0

var pressed:bool = false
@onready var startPos = global_position
var index = -1

func _ready() -> void:
	if OS.get_name() == "Android":
		show()

func _on_button_pressed() -> void:
	if !pressed:
		pass
	pressed = true


func _on_button_released() -> void:
	pressed = false
	index = -1
	global_position = startPos
	$Center.global_position = startPos

func _input(event: InputEvent) -> void:
	if pressed and event is InputEventScreenTouch:
		if index==-1:
			index = event.index
			global_position = event.position - Vector2(maxLength,maxLength)
		if(index==event.index):
			var getVector = event.position - global_position - Vector2(maxLength,maxLength)
			if(getVector.length()>maxLength):
				getVector = getVector.normalized() * maxLength
			$Center.global_position = global_position + getVector
			if getVector.x>maxLength/2:
				Input.action_press("MoveRight")
				await get_tree().create_timer(1/Engine.get_frames_per_second()).timeout
				Input.action_release("MoveRight")
			if getVector.x<maxLength/2:
				Input.action_press("MoveLeft")
				await get_tree().create_timer(1/Engine.get_frames_per_second()).timeout
				Input.action_release("MoveLeft")
			if getVector.y>maxLength/2:
				Input.action_press("MoveDown")
				await get_tree().create_timer(1/Engine.get_frames_per_second()).timeout
				Input.action_release("MoveDown")
			if getVector.y<maxLength/2:
				Input.action_press("MoveUp")
				await get_tree().create_timer(1/Engine.get_frames_per_second()).timeout
				Input.action_release("MoveUp")
