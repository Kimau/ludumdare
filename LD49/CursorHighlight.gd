extends Sprite


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	get_parent().connect("highlight", self, "_on_highlight")
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass

func _on_highlight(pos):
	if pos == null:
		self.visible = false
	else:
		position = pos + self.get_rect().size * 0.5
		self.visible = true		
