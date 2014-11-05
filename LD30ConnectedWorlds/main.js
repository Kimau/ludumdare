// Core Varibles
var gameCanvas;
var ctx;
var ticks = 0;
var bgcolor = "#000";
var boxWidth = 700;
var boxHeight = 600;
var currScene = 'flying';

// Transition
var transState;
var transTime = 5.0;

// Debug
var oX = -20;
var oY = -20;
var oR = 1;

// Game Resources
var time = 0;
var deltaT = 1.0 / 60.0;
var inputState = 
{
	"Brakes" : 0,
	"Thrusters" : 0,
	"Turn" : 0
}
var gameState =
{
	"Ship": { "Pos": [0,0], "Vel": [0,0], "Acl": [0,0], "Face": [0,0]}
}
var particles = [];

//------------------------------------------------------------------------
// Helper Functions
function $(id) { return document.getElementById(id); }
function log(m) { "use strict"; $("debugLog").innerHTML = m + "\n" + $("debugLog").innerHTML; }
function len(pt) { return Math.sqrt(pt[0] * pt[0] + pt[1] * pt[1]); }
function deltaSeg(a) { return [a[2] - a[0], a[3] - a[1]]; }
function deltaPts(a, b) { return [b[0] - a[0], b[1] - a[1]]; }
function clearLive() { $("liveOutput").innerHTML = ""; }

function live(label, val, longLabel) {
	if(typeof(val) == "number")
	{
		val = val.toFixed(2);
	}

	if($("lo_" + label) == undefined)
	{
		if(longLabel == undefined) { longLabel = label; }
		$("liveOutput").innerHTML += "<dt>" + longLabel + '</dt><dd id="lo_' + label + '">' + val + '</dd>';
	}

	$("lo_" + label).innerHTML = val;
}

function setBox(width, height)
{
	if(typeof(width) != "number") { width = 400; }
	if(typeof(height) != "number") { height = 300; }

	boxWidth = width
	boxHeight = height;

	gameCanvas.width = boxWidth;
	gameCanvas.height = boxHeight;

	mL = (window.innerWidth / 2) - (boxWidth / 2);
	gameCanvas.style["margin"] = "0px 0px 0px " + mL;
}

function randUnitVec()
{
	var d,nx,ny;

	nx = Math.random() - 0.5;
	ny = Math.random() - 0.5;
	d = len([nx,ny]);
	nx = nx / d;
	ny = ny / d;
	return [nx,ny]
}

function randPosVec(minD, maxD)
{
	var d,v;

	d = Math.random() * (maxD - minD) + minD;
	v = randUnitVec();
	return [v[0]*d, v[1]*d];
}

function getAngle(x,y)
{
	var a;
	a = Math.atan2(x, y)
	if(a < 0) { a += Math.PI*2; }
        a += Math.PI / 2;

	return a;
}

function Det(a,b)
{
	return a[0]*b[1] - a[1]*b[0];
}

function IsInside(a,b,c,p)
{
	b = [b[0] - a[0], b[1] - a[1]];
	c = [c[0] - a[0], c[1] - a[1]];
	p = [p[0] - a[0], p[1] - a[1]];

	var den  = Det(b,c);
	var num1 = Det(p,b) / den;
	var num2 = Det(p,b) / den;

	return ((num1 > 0) && (num2 > 0) && ((num1 + num2) < 1));
}

function PointInTriangle(pt, v1, v2, v3)
{
	function sign(p1, p2, p3)
	{
		return (p1[0] - p3[0]) * (p2[1] - p3[1]) - (p2[0] - p3[0]) * (p1[1] - p3[1]);
	}
	
	var b1, b2, b3;
	
	b1 = sign(pt, v1, v2) < 0.0;
	b2 = sign(pt, v2, v3) < 0.0;
	b3 = sign(pt, v3, v1) < 0.0;
	
	return ((b1 == b2) && (b2 == b3));
}

function IsIntersecting(segA, segB)
{
	var denominator = ((segA[2] - segA[0]) * (segB[3] - segB[1])) - ((segA[3] - segA[1]) * (segB[2] - segB[0]));
	var numerator1  = ((segA[1] - segB[1]) * (segB[2] - segB[0])) - ((segA[0] - segB[0]) * (segB[3] - segB[1]));
	var numerator2  = ((segA[1] - segB[1]) * (segA[2] - segA[0])) - ((segA[0] - segB[0]) * (segA[3] - segA[1]));

	// Detect coincident lines (has a problem, read below)
	if(denominator === 0)
		return (numerator1 === 0) && (numerator2 === 0);

	numerator1 = numerator1 / denominator;
	numerator2 = numerator2 / denominator;

	return (numerator1 >= 0 && numerator1 <= 1) && (numerator2 >= 0 && numerator2 <= 1);
}

function BatchIntersectingLines(listSegs, seg)
{
	for(var i=0; i<listSegs.length;++i)
		if(IsIntersecting(listSegs[i],seg) === true)
			return true;

	return false;
}

function BatchIntersectingPinLines(seg)
{
	for(var p in gameLines)
		if(BatchIntersectingLines(gameLines[p],seg) === true)
			return true;

	return false;
}

function GetClosetPoint(pt, line, isClamped)
{
	var t, delta;
	delta = deltaSeg(line);

	pt = [pt[0] - line[0], pt[1] - line[1]];

	t = (pt[0]*delta[0] + pt[1]*delta[1]) / (delta[0]*delta[0] + delta[1]*delta[1]);
	if(isClamped === true)
	{
		if (t < 0) 
			t = 0.0;
		else if (t > 1) 
			t = 1.0;
	}

	return [line[0] + (delta[0] * t), line[1] + (delta[1] * t)];
}

function Factorial(x)
{
	var c = 1;
	while(x > 0)
		c = c * x--;

	return c;
}

//------------------------------------------------------------------------
//                                                    ___               
//                                                   / __\___  _ __ ___ 
//                                                  / /  / _ \| '__/ _ \
//                                                 / /__| (_) | | |  __/
//                                                 \____/\___/|_|  \___|
//------------------------------------------------------------------------
function gameTick()
{
	ticks++;
	processKeyboard();
	updateLogic();

	// Need to look into seperating the Render off into using updateAnimation
	drawScene();
}

function initGame()
{
	// Get Context
	gameCanvas = $("gameCanvas");
	setBox(boxWidth, boxHeight);
	ctx = gameCanvas.getContext("2d");
	gameCanvas.onmousedown = gameMouseDown;
	gameCanvas.onmouseup = gameMouseUp;
	gameCanvas.onmousemove = gameMouseMove;

	// Load Resources

	// Setup Game Logic

	// Start Game
	setInterval(gameTick, 1000/30); 

	window.onkeydown = keyDown;
	window.onkeyup = keyUp;
	window.onresize = function() { setBox(boxWidth, boxHeight); };

	log("Game Started");
}

//------------------------------------------------------------------------
//                     ___                         __             _      
//                    / _ \__ _ _ __ ___   ___    / /  ___   __ _(_) ___ 
//                   / /_\/ _` | '_ ` _ \ / _ \  / /  / _ \ / _` | |/ __|
//                  / /_\\ (_| | | | | | |  __/ / /__| (_) | (_| | | (__ 
//                  \____/\__,_|_| |_| |_|\___| \____/\___/ \__, |_|\___|
//                                                          |___/        
//------------------------------------------------------------------------
function updateLogic()
{
	time += deltaT;
	switch(currScene)
	{
		case 'flying': updateFlying();
	}

	//. . . . . . . . . . . . . . .
	// Handle Transition 
	if(transTime == undefined)
	{
		return;
	}

	if(transState == 'out')
	{
		transTime -= 0.1;

		if(transTime < 0)
		{
			transTime = 0;
			nextScene();
		}
	}
	else if(transState == 'in')
	{
		transTime += 0.1;

		if(transTime > 5)
		{
			transTime = 5;
			transState = undefined;
		}
	}
}

var partTime = 0.0;
function updateFlying()
{
	var deltaT = 1.0 / 30.0;
	
	//==========================================================
	// Particle Drift
	function partDead(p) { return p[2] > deltaT; }
	particles = particles.filter(partDead);
	
	partTime += deltaT + len(gameState.Ship.Vel) * deltaT;
	if(partTime > 10.0)
	{
		partTime = 0.0;
		var np = randUnitVec();
		np[0] = gameState.Ship.Pos[0] + np[0] * 20.0;
		np[1] = gameState.Ship.Pos[1] + np[1] * 20.0;
		np[2] = 2;
		particles.push(np);
	}	

	for(var i=0; i<particles.length; ++i)
		particles[i][2] -= deltaT;
	//==========================================================
	
	gameState.Ship.Face[1] += inputState.Turn * 10.0 * deltaT;
	gameState.Ship.Face[1] = Math.min(10.0,Math.max(-10.0,gameState.Ship.Face[1]));
	gameState.Ship.Face[0] += gameState.Ship.Face[1] * deltaT;
	
	gameState.Ship.Acl[0] = Math.sin(gameState.Ship.Face[0]) * inputState.Thrusters * -10.0;
	gameState.Ship.Acl[1] = Math.cos(gameState.Ship.Face[0]) * inputState.Thrusters * 10.0;

	gameState.Ship.Vel = [
		gameState.Ship.Vel[0] + gameState.Ship.Acl[0] * deltaT,
		gameState.Ship.Vel[1] + gameState.Ship.Acl[1] * deltaT];
	
	if(inputState.Brakes > 0)
	{
		gameState.Ship.Vel = [
			gameState.Ship.Vel[0] - gameState.Ship.Vel[0] * deltaT,
			gameState.Ship.Vel[1] - gameState.Ship.Vel[1] * deltaT];
		gameState.Ship.Face[1] = gameState.Ship.Face[1] - gameState.Ship.Face[1] * deltaT;
	}
	
	gameState.Ship.Pos = [
		gameState.Ship.Pos[0] + gameState.Ship.Vel[0] * deltaT,
		gameState.Ship.Pos[1] + gameState.Ship.Vel[1] * deltaT];
	
	
	live('ShipFace', gameState.Ship.Face[0], 'Ship Face');
	live('ShipVelX', gameState.Ship.Vel[0], 'Ship Vel X');
	live('ShipVelY', gameState.Ship.Vel[1], 'Ship Vel Y');	
	live('ShipPosX', gameState.Ship.Pos[0], 'Ship Pos X');
	live('ShipPosY', gameState.Ship.Pos[1], 'Ship Pos Y');
}

function nextScene(jmpTo)
{
	transTime = 0;
	transState = 'in';

	if(jmpTo == undefined)
	{
		switch(currScene)
		{
		}
	}

	switch(jmpTo)
	{
	}

	currScene = jmpTo;
	clearLive();  // Clear Live Debug Output
}

//------------------------------------------------------------------------
//                                             _____                   _   
//                                             \_   \_ __  _ __  _   _| |_ 
//                                              / /\/ '_ \| '_ \| | | | __|
//                                           /\/ /_ | | | | |_) | |_| | |_ 
//                                           \____/ |_| |_| .__/ \__,_|\__|
//                                                        |_|             
//------------------------------------------------------------------------ 
function processKeyboard()
{
	// Do Stuff 
}

function keyDown(e)
{
	var handled = false;
	var code = e.which || e.keyCode;

	switch(code)
	{
		case 17:
			inputState.Brakes = 1;
			handled = true; break; // Ctrl
		case 32:
			inputState.Thrusters = 20;
			handled = true; break; // Space
		case 65:
		case 37: 
			inputState.Turn = -1.0;
			handled = true; break; // Left
		case 38: 
		case 87:
			inputState.Thrusters = +1.0;
			haandled = true; break; // Up
		case 68:
		case 39: 
			inputState.Turn = +1.0;
			handled = true; break; // right
		case 40:
		case 83:
			inputState.Thrusters = -1.0;
			handled = true; break; // down
	}

	var kStr = String.fromCharCode(code);
	switch(kStr)
	{
	}


	if (handled && e.preventDefault)
	{
		// prevent from browser to receive this event
		e.preventDefault();
	}
}

function keyUp(e)
{
	var handled = false;
	var code = e.which || e.keyCode;

	switch(code)
	{
		// DEBUG
		//
		case 17:
			inputState.Brakes = 0;
			handled = true; break; // Ctrl
		case 32:
			inputState.Thrusters = 0;
			handled = true; break; // Space
		case 65:
		case 37: 
			inputState.Turn += 1.0;
			handled = true; break; // Left
		case 38: 
		case 87:
			inputState.Thrusters = 0;
			handled = true; break; // Up
		case 68:
		case 39: 
			inputState.Turn -= 1.0;
			handled = true; break; // right
		case 40:
		case 83:
			inputState.Thrusters = 0;
			handled = true; break; // down
		default:
			log(code);
	}

	var kStr = String.fromCharCode(code);
	switch(kStr)
	{
	}

	if (handled && e.preventDefault)
	{
		// prevent from browser to receive this event
		e.preventDefault();
	}
}

function gameMouseDown(e)
{
	mouseX = e.layerX;
	prevX = mouseX;

	e.preventDefault();
}

function gameMouseUp(e)
{
	e.preventDefault();
}

function gameMouseMove(e)
{
	var dH, dT;

	e.preventDefault();
}

//------------------------------------------------------------------------
//                                        __                _           
//                                       /__\ ___ _ __   __| | ___ _ __ 
//                                      / \/// _ \ '_ \ / _` |/ _ \ '__|
//                                     / _  \  __/ | | | (_| |  __/ |   
//                                     \/ \_/\___|_| |_|\__,_|\___|_|   
//------------------------------------------------------------------------
function drawScene()
{
	switch(currScene)
	{
		case 'flying': drawFlying();
	}
}

function drawFlying()
{
	var imgStarBG = $("imgGrid");
	var imgFlame = $("imgFlames");
	var imgBooster = $("imgBooster");
	var imgShip = $("imgShip");
	var imgShield = $("imgShield");
	var imgSpeck = $("imgSpeck");
	
	var zoomLevel = 100 / Math.max(100, (len(gameState.Ship.Vel) - 100));
	zoomLevel = Math.max(0.1,Math.min(1, zoomLevel));
	
	ctx.setTransform(1, 0, 0, 1, 0, 0);
	ctx.fillRect(0,0,boxWidth, boxHeight);
	
	ctx.save();
	ctx.translate(boxWidth*0.5, boxHeight*0.5);
	ctx.scale(zoomLevel, zoomLevel);
	

	function DrawGrid(spacing, size)
	{
		ctx.save();	
		ctx.translate(
				-gameState.Ship.Pos[0], 
				-gameState.Ship.Pos[1]);
				
		var offset = [
			Math.floor(gameState.Ship.Pos[0] / spacing)*spacing,
			Math.floor(gameState.Ship.Pos[1] / spacing)*spacing];
			
		var gridLines = [
			offset[0] - Math.floor(boxWidth *0.5/zoomLevel / spacing +1) * spacing,
			offset[1] - Math.floor(boxHeight*0.5/zoomLevel / spacing +1) * spacing,
			offset[0] + Math.floor(boxWidth *0.5/zoomLevel / spacing +2) * spacing,
			offset[1] + Math.floor(boxHeight*0.5/zoomLevel / spacing +2) * spacing,
			spacing,
			spacing
		];		

		ctx.beginPath();
		{
			ctx.strokeStyle="purple";
			ctx.lineWidth=size*zoomLevel;
					
			for(var x=gridLines[0]; x<gridLines[2]+20; x+=gridLines[4])
			{
				ctx.moveTo(x,gridLines[1]);
				ctx.lineTo(x,gridLines[3]);
			}
			
			for(var y=gridLines[1]; y<gridLines[3]+20; y+=gridLines[5])
			{
				ctx.moveTo(gridLines[0],y);
				ctx.lineTo(gridLines[2],y);
			}
		}
		ctx.stroke();	
		ctx.restore();
	}
	
	DrawGrid(100,1);
	DrawGrid(500,6);
	DrawGrid(2500,14);
	
	
	ctx.save();
		// Particles
		ctx.translate(
				-gameState.Ship.Pos[0], 
				-gameState.Ship.Pos[1]);
		for(var i=0; i<particles.length; ++i)
		{
			ctx.globalAlpha = particles[i][2] / 10.0+0.1;
			ctx.drawImage(imgSpeck, 0,0,10,9,particles[i][0], particles[i][1], 
			Math.max(4,20 / Math.max(1,particles[i][2])), 
			Math.max(4,20 / Math.max(1,particles[i][2])));
		}
	ctx.restore();
	
	// Ship
	ctx.save();
		ctx.rotate(gameState.Ship.Face[0]);
		
		var l = len(gameState.Ship.Vel);
		var p = [1.0,0.1];
		for(var x=-100; x > p[0]; x -= p[1])
		{
			ctx.drawImage(imgSpeck, gameState.Ship.Vel[0] * x, gameState.Ship.Vel[1] * x);
		}
		
		// Forward Thrusters
		if(inputState.Thrusters > 0)
		{
		ctx.save();
			var offX = Math.floor(time * 60.0 / 3.0) % 5;
			//imgBooster
			if(inputState.Thrusters > 2)
			{
				ctx.translate(0, -37);		
				ctx.drawImage(imgBooster, 0,0,64,149,-32,-140,64,149);
			}
			else
			{
				ctx.translate(-23, -47);		
				ctx.drawImage(imgFlame, offX*10,0,10,20,-5,-10,10,20);
				ctx.translate(15, 0); offX = (offX+1) % 5;
				ctx.drawImage(imgFlame, offX*10,0,10,20,-5,-10,10,20);
				ctx.translate(16, 0); offX = (offX+1) % 5;
				ctx.drawImage(imgFlame, offX*10,0,10,20,-5,-10,10,20);
				ctx.translate(16, 0); offX = (offX+1) % 5;
				ctx.drawImage(imgFlame, offX*10,0,10,20,-5,-10,10,20);	
			}
		ctx.restore();
		}

		// Back Thruster
		if(inputState.Thrusters < 0)
		{
		ctx.save();
			var offX = Math.floor(time * 60.0 / 3.0) % 5;
			ctx.translate(2, 47);		
			ctx.rotate(Math.PI);
			ctx.drawImage(imgFlame, offX*10,0,10,20,-5,-10,10,20);
		ctx.restore();
		}
		
		// Turn Left
		if(inputState.Turn > 0)
		{
		ctx.save();
			var offX = Math.floor(time * 60.0 / 3.0) % 5;
			ctx.translate(28, 15);		
			ctx.rotate(Math.PI*0.7);
			ctx.drawImage(imgFlame, offX*10,0,10,20,-5,-10,10,20);
		ctx.restore();
		}
		
		// Turn Right
		if(inputState.Turn < 0)
		{
		ctx.save();
			var offX = Math.floor(time * 60.0 / 3.0) % 5;
			ctx.translate(-26, 16);		
			ctx.rotate(Math.PI*1.3);
			ctx.drawImage(imgFlame, offX*10,0,10,20,-5,-10,10,20);
		ctx.restore();
		}

		ctx.drawImage(imgShip, -imgShip.width*0.5,-imgShip.height*0.5);
		
		// Brakes
		if(inputState.Brakes > 0)
		{
			ctx.save();
				ctx.drawImage(imgShield, imgShield.naturalWidth * -0.5, imgShield.naturalHeight * -0.5);
			ctx.restore();
		}
	ctx.restore();
	ctx.restore();
}

