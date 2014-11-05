// Ludum Dare 25: _________ by Claire Blackshaw (Kimau)
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
var currScene;

// Transition
var transState;
var transTime = 5.0;

// Game Resources

//------------------------------------------------------------------------
// Helper Functions
function $(id) { return document.getElementById(id); }
function Log(m) { "use strict"; $("debugLog").innerHTML += m + "\n"; }
function Len(x, y) { return Math.sqrt(x * x + y * y); }
function ClearLive() { $("liveOutput").innerHTML = ""; }

function Live(label, val, longLabel) 
{
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

function SetBox(width, height)
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

function RandUnitVec()
{
	var d,nx,ny;

	nx = Math.random() - 0.5;
	ny = Math.random() - 0.5;
	d = len(nx,ny);
	nx = nx / d;
	ny = ny / d;
	return [nx,ny]
}

function RandPosVec(minD, maxD)
{
	var d,v;

	d = Math.random() * (maxD - minD) + minD;
	v = RandUnitVec();
	return [v[0]*d, v[1]*d];
}

function GetAngle(x,y)
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
function GameTick()
{
	ticks++;
	ProcessKeyboard();
	UpdateLogic();

	// Need to look into seperating the Render off into using updateAnimation
	DrawScene();
}

function InitGame()
{
	// Get Context
	gameCanvas = $("gameCanvas");
	SetBox(400,300);
	ctx = gameCanvas.getContext("2d");
	gameCanvas.onmousedown = GameMouseDown;
	gameCanvas.onmouseup   = GameMouseUp;
	gameCanvas.onmousemove = GameMouseMove;

	// Load Resources

	// Setup Game Logic
	NextScene('intro');

	// Start Game
	setInterval(GameTick, 1000/30); 

	window.onkeydown = KeyDown;
	window.onkeyup = KeyUp;
	window.onresize = function() { SetBox(400,300); };

	Log("Game Started");
}

//------------------------------------------------------------------------
//                     ___                         __             _      
//                    / _ \__ _ _ __ ___   ___    / /  ___   __ _(_) ___ 
//                   / /_\/ _` | '_ ` _ \ / _ \  / /  / _ \ / _` | |/ __|
//                  / /_\\ (_| | | | | | |  __/ / /__| (_) | (_| | | (__ 
//                  \____/\__,_|_| |_| |_|\___| \____/\___/ \__, |_|\___|
//                                                          |___/        
//------------------------------------------------------------------------
function UpdateLogic()
{
	currScene.Update();
	
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
			NextScene();
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

function NextScene(jmpTo)
{
	transTime = 0;
	transState = 'in';

	if(jmpTo == undefined)
	{
		switch(currScene)
		{
			case 'intro':   jmpTo = currScene.nextScene;   break;
		}
	}

	switch(jmpTo)
	{
		case 'intro':   currScene = new StartIntroScene();     break;
		default: alert("Don't know what to jump to [" + jmpTo + "]");
	}

	ClearLive();  // Clear Live Debug Output
}

function StartIntroScene()
{
	this.Update = function() { alert("Update"); };
	this.Render = function() { alert("Render"); };
	this.Input  = function() { alert("Input");  };
}

//------------------------------------------------------------------------
//                                             _____                   _   
//                                             \_   \_ __  _ __  _   _| |_ 
//                                              / /\/ '_ \| '_ \| | | | __|
//                                           /\/ /_ | | | | |_) | |_| | |_ 
//                                           \____/ |_| |_| .__/ \__,_|\__|
//                                                        |_|             
//------------------------------------------------------------------------ 
function ProcessKeyboard()
{
	// Do Stuff 
}

function KeyDown(e)
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

function KeyUp(e)
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
		case 'A': handled = true; break;
		case 'D': handled = true; break;
	}

	if (handled && e.preventDefault)
	{
		// prevent from browser to receive this event
		e.preventDefault();
	}
}

function GameMouseDown(e)
{
	e.preventDefault();
}

function GameMouseUp(e)
{
	e.preventDefault();
}

function GameMouseMove(e)
{

	if(e.which == 1)
	{
	}

	switch(currScene)
	{
		case 'intro': 
			break;	
	}

	e.preventDefault();
}

//------------------------------------------------------------------------
//                                        __                _           
//                                       /__\ ___ _ __   __| | ___ _ __ 
//                                      / \/// _ \ '_ \ / _` |/ _ \ '__|
//                                     / _  \  __/ | | | (_| |  __/ |   
//                                     \/ \_/\___|_| |_|\__,_|\___|_|   
//------------------------------------------------------------------------
function DrawScene()
{
	ctx.setTransform(1, 0, 0, 1, 0, 0);
	currScene.Render();

	// Draw Transition
	if(transTime < 5)
	{
		var i = 0;
		var s = 40.0;
		var a = Math.min(1.0, 1.0 - (transTime * 0.1));

		if(transTime > 1.0)
		{
			s = Math.min(40, (5 - transTime) * 10);
		}

		
		while(i < 90)
		{
			ctx.save();
			if((i%2) == 0)
				ctx.fillStyle = "rgba(200,200,200," + a + ")";
			else  
				ctx.fillStyle = "rgba(100,100,100," + a + ")";

			ctx.translate((i % 11) * 40, Math.floor(i / 11) * 40);
			ctx.rotate( Math.max(0,(transTime - 0.8)*2.0) );
			ctx.fillRect(-(s/2), -(s/2), s, s);
			ctx.restore();
			i += 1;
		}
	}
}
