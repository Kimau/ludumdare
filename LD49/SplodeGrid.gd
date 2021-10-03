extends TileMap
class_name SplodeGrid

signal highlight

var highlight = null
var splodCol = 0
var boardSize = [10,14]
var rng = RandomNumberGenerator.new()
var countdown = 30.0
var splodCount = 0

const countdown_max = 15.0
const colors = [
	Color(1.0,0.0,0.0,1.0),
	Color(0.0,1.0,0.0,1.0),
	Color(0.0,0.0,1.0,1.0),
]
const nPos = [[-1,0],[1,0],[0,-1],[0,1]]

# Called when the node enters the scene tree for the first time.
func _ready():
	genBoard()

func _input(event):
	if "position" in event:
		var tilePos = self.world_to_map(self.to_local(event.position))
		var idx = self.get_cellv(tilePos)
		if idx < 0:
			setHighlight(null)
		else:
			setHighlight(tilePos)
			
	if event is InputEventKey and (event.pressed):
		if(event.scancode == KEY_O):
			genBoard()
		elif(event.scancode == KEY_P):
			countdown = 0.1
		elif(event.scancode == KEY_A):
			slideHighlight(+1,0)
		elif(event.scancode == KEY_D):
			slideHighlight(-1,0)
		elif(event.scancode == KEY_W):
			slideHighlight(0,+1)
		elif(event.scancode == KEY_S):
			slideHighlight(0,-1)
	pass
	
func setHighlight(tilePos):
	if(countdown <= 0):
		tilePos = null
	highlight = tilePos
	if tilePos:
		emit_signal("highlight", self.map_to_world(tilePos))
	else:
		emit_signal("highlight", null)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	var a = self.map_to_world(Vector2(boardSize[0], boardSize[1]));
	self.position = (get_viewport().size / 2.0) - a * 0.5 * self.scale
	
	if countdown > 0:
		countdown = countdown - delta
	elif(countdown < 0):
		countdown = 0
		setHighlight(null)
	else:
		splodCount = splodCount - delta
		if(splodCount < 0):
			splodCount = 0.2
			ExplodeBoardTick()

func floodFill(x,y):
	var c = self.get_cell(x,y)
	for n in nPos:
		var cn = self.get_cell(x+n[0],y+n[1])
		if cn == 4:
			match rng.randi() % 5:
				0:
					self.set_cell(x+n[0],y+n[1], c)
				_:
					self.set_cell(x+n[0],y+n[1], (c+1)%3)
			floodFill(x+n[0],y+n[1])

func genBoard():
	self.clear()
	for x in boardSize[0]:
		for y in boardSize[1]:
			var c = rng.randi() % 35
			if c < 30:
				c = 4
			else:
				c = 3
			self.set_cell(x,y,c)
	
	splodCol = rng.randi() % 3
	var	rx = rng.randi() % boardSize[0]
	var	ry = rng.randi() % boardSize[1]
	self.set_cell(rx,ry,splodCol)
	floodFill(rx,ry)
	self.set_cell(rx,ry,13)
	
	# Fill Spots
	for x in boardSize[0]:
		for y in boardSize[1]:
			if self.get_cell(x,y) == 4:
				self.set_cell(x,y,3)
	
	# Random Slides
	for s in 50:
		var c = rng.randi() % (boardSize[0]*2 + boardSize[1]*2)
		if(c < boardSize[0]):
			slide(0,c,-1,0)
		elif(c < (boardSize[0]*2)):
			slide(0,int(c/2),+1,0)
		elif(c < (boardSize[0]*3)):
			slide(int(c/3),0,0,-1)
		else:
			slide(int(c/4),0,0,+1)
	
	countdown = countdown_max
	
func MapToIdx(x,y):
	return int(x + y*boardSize[0])

func ExplodeBoardTick():
	var cells = self.get_used_cells()
	var hotCells = {}
	var deadCells = {}
	
	# Find Hot Cells
	for c in cells:
		var v = self.get_cell(c.x, c.y)
		if v == 13:
			hotCells[MapToIdx(c[0], c[1])] = splodCol
		elif (v >= 10):
			self.set_cell(c[0],c[1],5+3)
		elif (v > 2) and ((v%5) < 3):
			hotCells[MapToIdx(c[0], c[1])] = v%5	
	
	# Explode One Colour Over
	var unstable = false
	for i in hotCells:
		var x = int(i % boardSize[0])
		var y = int(i / boardSize[0])
		var c = hotCells[i]
		
		
		var n = []
		for p in nPos:
			var idx = MapToIdx(x+p[0], y+p[1])
			var nc = self.get_cell(x+p[0], y+p[1])
			if hotCells.has(idx):
				pass
			elif (nc == ((c+1) % 3)):
				hotCells[idx] = nc
				unstable = true
	
	# Flood to same colour
	var floodMe = true
	while floodMe:
		floodMe = false
		for i in hotCells:
			var x = int(i % boardSize[0])
			var y = int(i / boardSize[0])
			var c = hotCells[i]
			
			var n = []
			for p in nPos:
				var idx = MapToIdx(x+p[0], y+p[1])
				var nc = self.get_cell(x+p[0], y+p[1])
				if hotCells.has(idx):
					pass
				elif (nc == c):
					hotCells[idx] = nc
					unstable = true
					floodMe = true
	
	# Explode Board
	for i in hotCells:
		var x = int(i % boardSize[0])
		var y = int(i / boardSize[0])
		var c = hotCells[i]
		
		if self.get_cell(x,y) < 3:
			self.set_cell(x,y,(c%5)+5)
		else:
			self.set_cell(x,y,(c%5)+10)
	
	return unstable

func GetCountdown():
	return countdown

func GetColour():
	return colors[splodCol]

func slide(px,py,x,y):
	if x != 0:
		var row = []
		for ix in boardSize[0]:
			var c = self.get_cell((ix+x+boardSize[0]) % boardSize[0], py)
			if(c == 13): # can't move explode tiles
				return
			elif(c == 3): # black tile doesn't move
				pass
			else:
				row.append(c)
		
		var iStep = 0
		for ix in boardSize[0]:
			if(self.get_cell(ix, py) < 3):
				self.set_cell(ix, py, row[iStep])
				iStep += 1
	
	if y != 0:
		var col = []
		for iy in boardSize[1]:
			var c = self.get_cell(px, (iy+y+boardSize[1]) % boardSize[1])
			if(c == 13): # can't move explode tiles
				return
			elif(c == 3): # black tile doesn't move
				pass
			else:
				col.append(c)
		
		var iStep = 0
		for iy in boardSize[1]:
			if(self.get_cell(px, iy) < 3):
				self.set_cell(px, iy, col[iStep])
				iStep += 1
	
func slideHighlight(x,y):
	if(highlight == null):
		return
		
	slide(highlight[0], highlight[1],x,y)
	
