// Ludum Dare 24: _____________________ by Claire Blackshaw (Kimau)
//
// Quick Thanks to http://www.network-science.de/ascii/ for ASCII Word Art Comments
// Much more readable in the minimap of Sublime

//------------------------------------------------------------------------
// Core Varibles
var gameCanvas;
var ctx;
var ticks = 0;
var bgcolor = "#000";
var boxWidth = 400;
var boxHeight = 300;
var currScene = 'egg';

// Transition
var transState;
var transTime = 5.0;

// Debug
var oX = -20;
var oY = -20;
var oR = 1;

// Game Resources
var rollingX = 0;

//------------------------------------------------------------------------
// Helper Functions
function $(id) { return document.getElementById(id); }
function log(m) { "use strict"; $("debugLog").innerHTML += m + "\n"; }
function len(x, y) { return Math.sqrt(x * x + y * y); }
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
	d = len(nx,ny);
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
	setBox(400,300);
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
	window.onresize = function() { setBox(400,300); };

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
	rollingX += 2.7;
	while(rollingX > 200)
	{
		rollingX -= 200;
	}

	switch(currScene)
	{
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
		case 37: handled = true; break; // Left
		case 38: handled = true; break; // Up
		case 39: handled = true; break; // right
		case 40: handled = true; break; // down
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
		case 37: handled = true; break; // Left
		case 38: handled = true; break; // Up
		case 39: handled = true; break; // right
		case 40: handled = true; break; // down
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
	ctx.setTransform(1, 0, 0, 1, 0, 0);

	ctx.save();
		var fImg = $("imgFloor");
		ctx.translate(rollingX, boxHeight - 100);
		ctx.drawImage(fImg,-200,0);
		ctx.drawImage(fImg,   0,0);
		ctx.drawImage(fImg, 200,0);
		ctx.drawImage(fImg, 400,0);		
	ctx.restore();
}

