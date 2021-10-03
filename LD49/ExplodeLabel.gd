extends RichTextLabel


# Declare member variables here. Examples:
# var a = 2
# var b = "text"
export var tileGridPath : NodePath
var tileGrid : SplodeGrid

# Called when the node enters the scene tree for the first time.
func _ready():
	tileGrid = get_node(tileGridPath) as SplodeGrid
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if tileGrid:
		var col = tileGrid.GetColour()
		self.add_color_override("default_color", col)
		self.add_color_override("shadow_color", col * 0.2)
		
		var c = tileGrid.GetCountdown()
		if c > 0:
			self.text = ("%5.01f" % tileGrid.GetCountdown())
		else:
			self.text = "BOOM!"
#	pass
