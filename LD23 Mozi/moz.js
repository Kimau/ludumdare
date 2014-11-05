// Ludum Dare 23: Tiny World by Claire Blackshaw (Kimau)
//
//                 __  ___            _    __    _ ____   
//                /  |/  /___  ____  (_)  / /   (_) __/__ 
//               / /|_/ / __ \/_  / / /  / /   / / /_/ _ \
//              / /  / / /_/ / / /_/ /  / /___/ / __/  __/
//             /_/  /_/\____/ /___/_/  /_____/_/_/  \___/ 
//                                                      
//        _,  _,_ __, _,_ _, _   __,  _, __, __,    _,  _,  
//        |   | | | \ | | |\/|   | \ /_\ |_) |_    ~ ) ~_) .
//        | , | | |_/ | | |  |   |_/ | | | \ |      /    ) .
//        ~~~ `~' ~   `~' ~  ~   ~   ~ ~ ~ ~ ~~~   ~~~ ~~   
//                                                          
//        ___ _ _, _ , _   _  _  _, __, _,  __,
//         |  | |\ | \ |   |  | / \ |_) |   | \
//         |  | | \|  \|   |/\| \ / | \ | , |_/
//         ~  ~ ~  ~   )   ~  ~  ~  ~ ~ ~~~ ~  
//                    ~'                         
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

// Egg Scene
var egg;
var mouseX = 0.0;
var prevX = 0.0;

// Nom Time
var nom;

// Hatch
var hatch;
var flapWings = 0;
var spin = 0;

// Sunset
var sunset;
var poofCloud = 0;

// Debug
var oX = -20;
var oY = -20;
var oR = 1;

// Game Resources

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
	egg = new StartEggScene();
	nom = new StartNomScene();
	hatch = new StartHatchScene();
	sunset = new StartSunsetScreen();

	nextScene('egg');

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
	switch(currScene)
	{
		case 'egg': 	updateEgg(); break;
		case 'nom': 	updateNom(); break;
		case 'hatch':	updateHatch(); break;
		case 'sunset':  updateSunset(); break;
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
			case 'egg':   jmpTo = 'nom';   break;
			case 'nom':   jmpTo = 'hatch'; break;
			case 'hatch': jmpTo = 'sunset';break;
			case 'sunset':jmpTo = 'egg';   break;
		}
	}

	switch(jmpTo)
	{
		case 'egg':   egg =   new StartEggScene();     break;
		case 'nom':   nom =   new StartNomScene();     break;
		case 'hatch': hatch = new StartHatchScene();   break;
		case 'sunset':sunset= new StartSunsetScreen(); break;
	}

	currScene = jmpTo;
	clearLive();  // Clear Live Debug Output
}

function updateEgg()
{
	live("eggOpen",  	egg.eggOpen);
	live("swing",   	egg.swing);
	live("currVel", 	egg.currVel);
	live("currRot", 	egg.currRot);

	egg.swing += (mouseX - prevX);
	prevX = mouseX;

	if(Math.abs(egg.swing) > 0.1)
	{
		if((egg.currVel > 0) == (egg.swing < 0))
		{
			if(egg.eggOpen < 1.0)
			{
				if( (Math.abs(egg.currVel) > 0.5) && 
					(Math.abs(egg.swing) > 30) )
				{
					egg.eggOpen += 0.1;
					egg.eggOpen = Math.min(egg.eggOpen, 1.0);
				}
			}
			else
			{
				egg.outOfEgg -= 1;
				setWormPos(egg.worm, Math.sin(egg.outOfEgg * 0.03) * egg.outOfEgg, egg.outOfEgg);
			}
		}


		if(egg.eggOpen < 1.0)
		{
			egg.currVel = egg.currVel + 0.1 * (Math.min(30,Math.abs(egg.swing)) * (egg.swing > 0)?1:-1);				
		}		
		egg.swing *= 0.7;
	}
	else
	{
		egg.swing = 0;
	}

	if(Math.abs(egg.currVel) > 0.01)
	{
		egg.currRot += egg.currVel * 0.1;
		egg.currVel *= 0.9;
	}
	else
	{
		egg.currVel = 0;
	}

	if((egg.outOfEgg < -160) && (transState == undefined))
	{
		transState = 'out';
	}
}

function StartEggScene()
{
	this.type = "Egg Scene";
	this.swing = 0;
	this.currVel = 0;
	this.currRot = 0;
	this.eggOpen = 0;
	this.outOfEgg = 0;
	this.worm = new WormSprite();
	setWormPos(this.worm, 0,500);
	setWormPos(this.worm, 0,0);

	$("tutorial").innerHTML = "<h2>Break Out the Egg</h2>  \
	<ul>  \
	  <li>Swing Back and Forth</li>  \
	  <li>Get Enough Momentum to Crack Open</li>  \
	</ul>";
}

function colCheckWorm(w, f)
{
	return function(p)
	{
		var nd, x, y;

		x = p[0] + f.x;
		y = p[1] + f.y;

		nd = len(x - w.loc[0][0], y - w.loc[0][1]);
		if(nd < 40)
		{
			log("Nom");
			nom.nomCount += 1;

			if(nom.nomCount < 8)
			{
				return false;
			}

			nom.nomCount = 0;

			var n = 1;
			while(n < nom.worm.bID.length)
			{
				//this.bits = [[0,0,30],[0,60,28],[0,120,25],[0,180,20],[60,0,15],[60,60,10],[60,120,5]];

				if(nom.worm.bID[n] <= 1)
				{
					n += 1;
				}
				else if( (n < (nom.worm.bID.length - 1)) && (nom.worm.bID[n] > (nom.worm.bID[n+1]-1)) )
				{
					n += 1;
				}
				else
				{
					nom.worm.bID[n] -= 1;
					log("We made " + n + " into piece " + nom.worm.bID[n]);
					return false;
				}
			}

			// All Full Up
			allFullUp();
			nom.nomCount = 100;
			return true;
		}

		return true;
	}
}

function allFullUp()
{
	if(nom.allFullUp)
	{
		return;
	}

	$("tutorial").innerHTML = "<h2>Wrap Up into Pupa</h2>  \
	<ul> \
	  <li>Wrap up Tight</li> \
	</ul>";

	log("All Full Up")
	nom.allFullUp = true;
}

function updateNom()
{
	var nx, ny, nd, crinkX, crinkY, crinkD, ox, oy;
	live("nombg", 	nom.bg);
	live("nomdir", 	nom.moveDir);

	ox = nom.worm.loc[0][0];
	oy = nom.worm.loc[0][1];

	// Can we Roll into a Ball
	if(nom.allFullUp)
	{
		nd = nom.worm.loc.length - 1;
		nd = len(ox - nom.worm.loc[nd][0], oy - nom.worm.loc[nd][1]);

		if(nd < 20)
		{
			transState = 'out';
			return;
		}
	}

	// Are we Too Far from Food
	for (var i = nom.food.length - 1; i >= 0; i--) {
		nd = len(nom.food[i].x - ox, nom.food[i].y - oy);

		if(nd > 1200)
		{
			nom.food[i] = new GenFood(ox,oy);
		}
		else if(nd < 400) // On Screen do Collision
		{
			nom.food[i].pieces = nom.food[i].pieces.filter(colCheckWorm(nom.worm, nom.food[i]));

			if(nom.food[i].pieces.length == 0)
			{
				nom.food[i] = new GenFood(ox,oy);				
			}
		}
	};

	// Length, Unit and Clamp
	nd = len(nom.moveDir[0], nom.moveDir[1]);
	if(isNaN(nd) || (nd == 0))
	{
		nom.moveDir = [0,0];
	}
	else
	{
		nx = nom.moveDir[0] / nd;
		ny = nom.moveDir[1] / nd;
		nd = Math.min(200, nd);

		live("nomd", nd);

		// neckCrink
		crinkX = ox - nom.worm.loc[1][0];
		crinkY = oy - nom.worm.loc[1][1];
		crinkD = len(crinkX, crinkY);
		crinkX = crinkX / crinkD - nx;
		crinkY = crinkY / crinkD - ny;

		crinkD = len(crinkX, crinkY) * nd * 0.3;

		live("nomCrink", crinkD);

		nx = ox + nx * crinkD;
		ny = oy + ny * crinkD;

		setWormPos(nom.worm, nx, ny);

		// Update Background
		nom.bg[0] = nx - 200;
		nom.bg[1] = ny - 150;

		nom.moveDir[0] *= 0.8;
		nom.moveDir[1] *= 0.8;
	}
}

function GenFood(x,y)
{
	function randFoodPiece()
	{
		var f;
		f = randPosVec(20,80);
		return [f[0],f[1], Math.floor(Math.random() * 8)];
	}

	var d,nVec;
	nVec = randPosVec(500,800);

	this.type = "Nom Food";
	this.x = nVec[0] + x;
	this.y = nVec[1] + y;
	this.pieces = []
	for (var i = 0; i < 8; ++i) {
		this.pieces.push(randFoodPiece());
	};
}

function StartNomScene()
{
	this.type = "Nom Scene";
	this.bg = [0,0];
	this.nomCount = 0;
	this.allFullUp = false;
	this.food = [new GenFood(0,0), new GenFood(0,0), new GenFood(0,0), new GenFood(0,0), new GenFood(0,0)];
	this.worm = new WormSprite();
	this.moveDir = [1,1];
	setWormPos(this.worm, 200,500);
	setWormPos(this.worm, 200,0);

	$("tutorial").innerHTML = "<h2>Feed on Algae</h2>   \
	<ul>  \
	  <li>Wiggle Back and Forth</li>  \
	  <li>Find Food/li>  \
	</ul>";
}

function WormSprite()
{
	this.type = "worm sprite";
	this.bits = [[0,0,30],[0,60,28],[0,120,25],[0,180,20],[60,0,15],[60,60,10],[60,120,5]];
	this.loc = [[0,0],[0,0],[0,0],[0,0],[0,0],[0,0]];
	this.bID = [0,1,2,3,4,5];
}

function MoziSprite()
{
	this.type = "mozi sprite";
	this.mHead    = [174,103,52, 46,-40,-30];
	this.mBody    = [351,103,49,143,-20,-15];
	this.mWingTop = [242,103,47,142,-30,-15];
	this.mWingBot = [290,103,60,142,-30,-15];
	this.mLegTop  = [[174,150,21,50,-12,-10],[196,150,20,50,-10,-10],[217,150,24,50,-10,-10]];
	this.mLegBot  = [[174,201,21,44,-12,-10],[196,201,20,44,-10,-10],[217,201,24,44,-10,-10]];
	this.mEgg     = [ 70,103,10,27, -5, -5];

	this.headAng = [0, +0.5,+1.5]; 
	this.wingAng = [0, -0.2,-1.6]; 
	this.topA    = [0, +1.6,-0.0]; 
	this.topB    = [0, +1.6,-0.0]; 
	this.topC    = [0, +1.6,-0.0]; 
	this.botA    = [0, +0.2,-0.9]; 
	this.botB    = [0, +0.2,-0.9]; 
	this.botC    = [0, +0.2,-0.9];

	this.wingFlap = 0;
}

function updateHatch()
{
	live("emerge", hatch.emerging);
	live("eggOpen", hatch.eggOpen);
	live("botA", hatch.mozi.topA[0]);
	live("botB", hatch.mozi.topB[0]);
	live("botC", hatch.mozi.topC[0]);

	if(hatch.emerging > 1.2)
	{
		transState = 'out';
		return;
	}

	hatch.eggOpen = Math.min(1.0, hatch.emerging * 3.0);
	hatch.mozi.topA[0] = Math.min(1.0, Math.max(0.0, 1.5 - hatch.emerging));
	hatch.mozi.topB[0] = Math.min(1.0, Math.max(0.0, 1.7 - hatch.emerging));
	hatch.mozi.topC[0] = Math.min(1.0, Math.max(0.0, 1.9 - hatch.emerging));

	hatch.mozi.botA[0] = Math.min(1.0, Math.max(0.0, 1.6 - hatch.emerging));
	hatch.mozi.botB[0] = Math.min(1.0, Math.max(0.0, 1.8 - hatch.emerging));
	hatch.mozi.botC[0] = Math.min(1.0, Math.max(0.0, 1.9 - hatch.emerging));

	flapWings += 2;
	hatch.mozi.wingAng[0] = (hatch.emerging < 0.8)?0.0:(Math.sin(flapWings) + 1.0)*0.5;
}

function StartHatchScene()
{
	this.type = "Hatch Scene";
	this.mozi = new MoziSprite();
	this.eggOpen = 0.0;
	this.emerging = 0.0;

	this.mozi.topA[0] = 1.0;
	this.mozi.topB[0] = 1.0;
	this.mozi.topC[0] = 1.0;


	$("tutorial").innerHTML = "<h2>Hatch from Pupa</h2>   \
	<ul>  \
	  <li>Struggle free from Pupa Husk</li>  \
	  <li>Click to Break Free</li>  \
	</ul>";
}


function updateSunset()
{
	live("distToHead", sunset.distToHead);
	live("distToTail", sunset.distToTail);
	live("errMargin", sunset.errMargin);
	live("eggSpew", sunset.eggSpew);

	flapWings += 2;
	sunset.mozi.wingAng[0] = (Math.sin(flapWings) + 1.0)*0.5;
	poofCloud -= 0.1;
}

function StartSunsetScreen()
{
	this.type = "Sunset Scene";
	this.mozi = new MoziSprite();
	this.eggSpew = 0;
	this.errMargin = 0;
	this.spewStarDist = 0;

	this.eggs = []; // x,y,r
	this.spewingEgg = [-900,0,0];

	this.distToHead = 0;
	this.distToTail = 0;

	this.headPt = [110,130];
	this.tailPt = [200,180];
	this.eggDir = [this.headPt[0] - this.tailPt[0], this.headPt[1] - this.tailPt[1]];

	this.mozi.botA[0] = 1;
	this.mozi.botB[0] = 1;
	this.mozi.botC[0] = 1;


	$("tutorial").innerHTML = "<h2>Laying Eggs</h2>   \
	<ul>  \
	  <li>Lay your Eggs before Sunset</li>  \
	  <li>Run the mouse from Head to Tail</li>  \
	</ul>";
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
		case 'A': eggOpen -= 0.02; eggOpen = Math.max(Math.min(eggOpen, 1.0),0.0); handled = true; break;
		case 'D': eggOpen += 0.02; eggOpen = Math.max(Math.min(eggOpen, 1.0),0.0); handled = true; break;
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

	if(currScene == 'sunset')
	{
		sunset.eggSpew = 0;
		sunset.distToHead = len(e.layerX - sunset.headPt[0], e.layerY - sunset.headPt[1]);
		sunset.distToTail = len(e.layerX - sunset.tailPt[0], e.layerY - sunset.tailPt[1]);
		sunset.errMargin = sunset.distToHead;
		sunset.spewStarDist = sunset.distToTail;

		sunset.spewingEgg = [sunset.tailPt[0] + (sunset.eggDir[0]*0.2), sunset.tailPt[1] + (sunset.eggDir[1]*0.2), -0.9];
	}

	e.preventDefault();
}

function gameMouseUp(e)
{
	e.preventDefault();

	if(currScene == 'hatch')
	{
		hatch.emerging += 0.03;
	}
	else if(currScene == 'sunset')
	{
		sunset.errMargin += sunset.distToTail;

		if(sunset.eggSpew > 0.9)
		{
			transState = 'out';
		}
		else
		{
			sunset.errMargin = 100;
			poofCloud = 1.0;
		}
	}
}

function gameMouseMove(e)
{
	var dH, dT;
	if((currScene == 'sunset') && (sunset.errMargin > 25.0))
	{
		sunset.eggSpew = 0;
		e.preventDefault();
		return;
	}

	if(e.which == 1)
	{
		if(currScene == 'sunset')
		{
			dH = len(e.layerX - sunset.headPt[0], e.layerY - sunset.headPt[1]);
			dT = len(e.layerX - sunset.tailPt[0], e.layerY - sunset.tailPt[1]);

			sunset.errMargin += Math.max(0, sunset.distToHead - dH);
			sunset.errMargin += Math.max(0, dT - sunset.distToTail);

			sunset.distToHead = dH;
			sunset.distToTail = dT;

			sunset.eggSpew = 1.0 - (dT / sunset.spewStarDist);

			if(sunset.eggSpew < 0.3)
			{
				sunset.spewingEgg = [sunset.tailPt[0] + (sunset.eggDir[0]*(0.3-sunset.eggSpew)), sunset.tailPt[1] + (sunset.eggDir[1]*(0.3-sunset.eggSpew)), -0.9];
			}
			else if(sunset.eggSpew < 0.6)
			{
				sunset.spewingEgg = [sunset.tailPt[0] - (sunset.eggDir[0]*(sunset.eggSpew-0.3)), sunset.tailPt[1] - (sunset.eggDir[1]*(sunset.eggSpew-0.3)), -0.9];
			}
			else
			{
				sunset.spewingEgg[2] = -0.9 + (sunset.eggSpew - 0.6) / 0.4;
			}
		}
	}

	mouseX = e.layerX;

	switch(currScene)
	{
		case 'egg': 
			break;
		case 'nom': 
			nom.moveDir[0] = (e.layerX - 200); 
			nom.moveDir[1] = (e.layerY - 150); 
			break;

		case 'hatch': 
			break;

		case 'sunset':
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
function drawScene()
{
	ctx.setTransform(1, 0, 0, 1, 0, 0);
	switch(currScene)
	{
		case 'egg': 	drawEggScene();    break;
		case 'nom': 	drawNomScene();    break;
		case 'hatch':	drawHatchScene();  break;
		case 'sunset':  drawSunsetScene(); break;
	}

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

function drawMozi(m, x, y, r)
{
	ctx.save();
	ctx.translate(x,y);
	ctx.rotate(r);

		// Bot Wing
		ctx.save();
		ctx.translate(5,20);
		ctx.rotate( (m.wingAng[0] * (m.wingAng[2]-m.wingAng[1])) + m.wingAng[1] );
		ctx.drawImage($("sprMozi"), m.mWingBot[0], m.mWingBot[1], m.mWingBot[2], m.mWingBot[3], m.mWingBot[4], m.mWingBot[5], m.mWingBot[2], m.mWingBot[3]);
		ctx.restore();

		// Body
		ctx.drawImage($("sprMozi"), m.mBody[0],    m.mBody[1],    m.mBody[2],    m.mBody[3], m.mBody[4], m.mBody[5], m.mBody[2],    m.mBody[3]);

		//--------------------------------------
		ctx.save();
		ctx.translate(-5,10);
		ctx.rotate( (m.topA[0] * (m.topA[2]-m.topA[1])) + m.topA[1] );
		ctx.drawImage($("sprMozi"), m.mLegTop[0][0], m.mLegTop[0][1], m.mLegTop[0][2], m.mLegTop[0][3], m.mLegTop[0][4], m.mLegTop[0][5], m.mLegTop[0][2], m.mLegTop[0][3]);

			ctx.save();
			ctx.translate(0,30);
			ctx.rotate( (m.botA[0] * (m.botA[2]-m.botA[1])) + m.botA[1] );
			ctx.drawImage($("sprMozi"), m.mLegBot[0][0], m.mLegBot[0][1], m.mLegBot[0][2], m.mLegBot[0][3], m.mLegBot[0][4], m.mLegBot[0][5], m.mLegBot[0][2], m.mLegBot[0][3]);
			ctx.restore();

		ctx.restore();

		//--------------------------------------
		ctx.save();
		ctx.translate(-5,35);
		ctx.rotate( (m.topB[0] * (m.topB[2]-m.topB[1])) + m.topB[1] );
		ctx.drawImage($("sprMozi"), m.mLegTop[1][0], m.mLegTop[1][1], m.mLegTop[1][2], m.mLegTop[1][3], m.mLegTop[1][4], m.mLegTop[1][5], m.mLegTop[1][2], m.mLegTop[1][3]);

			ctx.save();
			ctx.translate(0,35);
			ctx.rotate( (m.botB[0] * (m.botB[2]-m.botB[1])) + m.botB[1] );
			ctx.drawImage($("sprMozi"), m.mLegBot[1][0], m.mLegBot[1][1], m.mLegBot[1][2], m.mLegBot[1][3], m.mLegBot[1][4], m.mLegBot[1][5], m.mLegBot[1][2], m.mLegBot[1][3]);
			ctx.restore();

		ctx.restore();

		//--------------------------------------
		ctx.save();
		ctx.translate(-5,55);
		ctx.rotate( (m.topC[0] * (m.topC[2]-m.topC[1])) + m.topC[1] );
		ctx.drawImage($("sprMozi"), m.mLegTop[2][0], m.mLegTop[2][1], m.mLegTop[2][2], m.mLegTop[2][3], m.mLegTop[2][4], m.mLegTop[2][5], m.mLegTop[2][2], m.mLegTop[2][3]);

			ctx.save();
			ctx.translate(0,30);
			ctx.rotate( (m.botC[0] * (m.botC[2]-m.botC[1])) + m.botC[1] );
			ctx.drawImage($("sprMozi"), m.mLegBot[2][0], m.mLegBot[2][1], m.mLegBot[2][2], m.mLegBot[2][3], m.mLegBot[2][4], m.mLegBot[2][5], m.mLegBot[2][2], m.mLegBot[2][3]);
			ctx.restore();

		ctx.restore();

		// Head
		ctx.save();
		ctx.rotate( (m.headAng[0] * (m.headAng[2]-m.headAng[1])) + m.headAng[1] );
		ctx.drawImage($("sprMozi"), m.mHead[0],    m.mHead[1],    m.mHead[2],    m.mHead[3], m.mHead[4], m.mHead[5], m.mHead[2],    m.mHead[3]);
		ctx.restore();

		// Top Wing
		ctx.save();
		ctx.translate(5,20);
		ctx.rotate( (m.wingAng[0] * (m.wingAng[2]-m.wingAng[1])) + m.wingAng[1] );
		ctx.drawImage($("sprMozi"), m.mWingTop[0], m.mWingTop[1], m.mWingTop[2], m.mWingTop[3], m.mWingTop[4], m.mWingTop[5], m.mWingTop[2], m.mWingTop[3]);
		ctx.restore();

	ctx.restore();
}

function drawHatchScene()
{
	spin += 0.05;

	ctx.drawImage($("imgSunrise"),0,0);

	// Draw Egg Bot
	ctx.save();
	ctx.translate(150,240);
	ctx.drawImage($("sprMozi"), 70, 0,210,100, -56, -79,210,100);

	// Draw Mozi
	ctx.save();
	drawMozi(hatch.mozi, -80 * hatch.emerging, -150 * hatch.emerging, -0.4);
	ctx.restore();

	// Sunrise Hack
	ctx.save();
	ctx.setTransform(1, 0, 0, 1, 0, 0);
	ctx.drawImage($("imgSunriseHack"),0,0);
	ctx.restore();

	// Draw Egg Top
	ctx.save();
	ctx.rotate(-0.3 - (hatch.eggOpen * 0.5)); // Go Negative
	ctx.translate(-8,8);
	ctx.drawImage($("sprMozi"), 292, 0,58,100, -36, -85,58,100);
	ctx.restore();

	ctx.save();
	ctx.rotate(-0.7 + (hatch.eggOpen * 0.5)); // Go Positive
	ctx.translate(8,8);
	ctx.drawImage($("sprMozi"), 352, 0,48,100, -25, -86,48,100);
	ctx.restore();

	ctx.restore();

	// Draw Water
	ctx.drawImage($("sprMozi"), 0, 247,400,30, 0,180,400,30);
}

function drawEggScene()
{
	ctx.drawImage($("imgWaterBG"),0,0);

	// Draw Egg
	ctx.save();
	ctx.translate(170,90);
	ctx.rotate(egg.currRot);
	ctx.translate(0,-egg.outOfEgg);
	ctx.drawImage($("imgEggBotBK"),-30,-20);

	// Draw Larva
	drawWorm(egg.worm, ctx);

	// Draw Lid
	ctx.save();
	ctx.translate(-20,0);
	if(egg.eggOpen) { ctx.rotate(-egg.eggOpen*2.4); }	
	ctx.drawImage($("imgEggTop"),-5,-45);	

	ctx.fillStyle= bgcolor;
	ctx.fillRect(0,0, 4, 4);	
	ctx.restore();

	// Draw Egg Top
	ctx.drawImage($("imgEggBot"),-30,-20);
	ctx.restore();
}

var debugTest = 1;
var foodSpr = [[180,0],[180,60],[180,120],[180,180],[180,240],[240,0],[240,60],[240,120],[240,180],[240,240]];

function drawNomScene()
{
	// Draw Scrolling Water Background
	var bx, by, ox, oy;
	ctx.fillRect(0,0,400,300);

	var parScale = 0.5;

	ox = (-nom.bg[0] * parScale) % 600;
	bx = (-nom.bg[0] * parScale) + (nom.bg[0] < 0)?600:-600;

	oy = (-nom.bg[1] * parScale) % 450;
	by = (-nom.bg[1] * parScale) + (nom.bg[1] < 0)?450:-450;

	ctx.drawImage($("imgWaterScroll"), ox   , oy);
	ctx.drawImage($("imgWaterScroll"), ox-bx, oy);
	ctx.drawImage($("imgWaterScroll"), ox+bx, oy);
	ctx.drawImage($("imgWaterScroll"), ox   , oy-by);
	ctx.drawImage($("imgWaterScroll"), ox-bx, oy-by);
	ctx.drawImage($("imgWaterScroll"), ox+bx, oy-by);
	ctx.drawImage($("imgWaterScroll"), ox   , oy+by);
	ctx.drawImage($("imgWaterScroll"), ox-bx, oy+by);
	ctx.drawImage($("imgWaterScroll"), ox+bx, oy+by);

	ctx.save();
	ctx.translate( -nom.bg[0], -nom.bg[1] );

	// Draw Worm
	ctx.save();
	drawWorm(nom.worm, ctx);
	ctx.restore();

	// Draw Food
	var i,j,f,p,v,ox,oy,nd,a;
	ox = nom.worm.loc[0][0];
	oy = nom.worm.loc[0][1];
	for (i = nom.food.length - 1; i >= 0; i--) 
	{
		f = nom.food[i];

		// Draw Food Bits
		ctx.save();
		ctx.translate(f.x, f.y);
		ctx.fillStyle = "#0F0";

		for (j = f.pieces.length - 1; j >= 0; j--) {
			p = f.pieces[j];
			ctx.save();			
			ctx.translate(p[0], p[1]);
			a = foodSpr[p[2]];
			ctx.drawImage($("sprWorm"), a[0], a[1], 60,60,-30,-30,60,60)
			ctx.restore();
		};
		ctx.restore();

		// Draw Guidance Markers
		v = [nom.food[i].x - ox, nom.food[i].y - oy];
		nd = len(nom.food[i].x - ox, nom.food[i].y - oy);
		if(nd > 100)
		{
			a = getAngle(v[0], -v[1]);
			v[0] = ox + v[0] * (100 / nd);
			v[1] = oy + v[1] * (100 / nd);

			if(nd > 400)
				nd = 1.0; // Math.max(0, 1.0 - (700 - nd) / 300);
			else if(nd < 400)
				nd = (nd - 100)/300;
			else
				nd = 1.0; 

			ctx.fillStyle = "rgba( 0,100, 0," + nd.toFixed(2) + ")";

			nd = Math.PI * (0.8);
			ctx.beginPath();  
			ctx.arc(v[0], v[1], 20, a + nd, a - nd, false);
			ctx.fill();
		}
	};

	// Pop Scroll
	ctx.restore();	
}

function drawSunsetScene()
{
	spin += 0.05;

	ctx.save();
	ctx.drawImage($("imgSunset"),0,0);

	if(sunset.errMargin < 25)
	{
		ctx.save();
		ctx.translate(sunset.spewingEgg[0], sunset.spewingEgg[1])
		ctx.rotate(sunset.spewingEgg[2]);
		ctx.scale(2.0, 2.0);
		ctx.drawImage($("sprMozi"), sunset.mozi.mEgg[0], sunset.mozi.mEgg[1], sunset.mozi.mEgg[2], sunset.mozi.mEgg[3], sunset.mozi.mEgg[4], sunset.mozi.mEgg[5], sunset.mozi.mEgg[2], sunset.mozi.mEgg[3]);
		ctx.restore();
	}

	drawMozi(sunset.mozi, 110, 130, -1.0);

	// Draw Water Top
	ctx.drawImage($("sprMozi"), 0, 247,400,30, 0,180,400,30);

	//
	if(poofCloud > 0)
	{
		ctx.save();
		ctx.translate(sunset.spewingEgg[0], sunset.spewingEgg[1])
		ctx.fillRect(-poofCloud * 20, -poofCloud * 20, poofCloud * 40, poofCloud * 40);
		ctx.restore();
	}

	ctx.restore();	
}

function drawWorm(w,ctx)
{
	var i = w.bID.length - 1;

	// Egg Specific Code
	if((currScene == 'egg') && (egg.eggOpen < 0.9))
	{
		i = 1;
	}	

	for (; i >= 0; i--) 
	{
		ctx.save();
		ctx.translate(w.loc[i][0],w.loc[i][1]);
		if((currScene == 'egg') && (egg.eggOpen < 0.9))
		{
			var s = 0.6;

			if(egg.eggOpen > 0.3)
			{
				s = 0.6 + 0.4 * (egg.eggOpen - 0.3) / (0.9-0.3);
			}

			ctx.scale(s,s);		
		}
		else if((i == 0) && (w.loc[i][0] < w.loc[i+1][0]))
		{
			ctx.scale(-1.0,1.0);
		}

		ctx.drawImage($("sprWorm"), w.bits[w.bID[i]][0], w.bits[w.bID[i]][1],60,60,-30,-30,60,60);
		ctx.restore();
	};
};

function setWormPos(w,nX,nY)
{
	function unit(x,y) { var dist = len(x,y); return [(x/dist), (y/dist)]; }
	var i, uVec, dx, dy, rD, wb;

	w.loc[0][0] = nX;
	w.loc[0][1] = nY;

	i = 1;
	while(i < w.bID.length)
	{
		dx = w.loc[i][0] - w.loc[i-1][0];
		dy = w.loc[i][1] - w.loc[i-1][1];
		rD = len(dx,dy);  
		dx = dx / rD;
		dy = dy / rD;

		wb = w.bits[w.bID[i-1]];

		if(rD < wb[2] * 0.25) // Move Away
		{
			w.loc[i][0] = w.loc[i-1][0] - (dx * wb[2] * 0.25);
			w.loc[i][1] = w.loc[i-1][1] - (dy * wb[2] * 0.25);
		}
		else if(rD > w.bits[i-1][2] * 0.5) // Move Towards
		{
			if(rD < w.bits[i-1][2])
			{
				w.loc[i][0] += (dx * 0.1);
				w.loc[i][1] += (dy * 0.1);
			}
			else // Max Move
			{
				w.loc[i][0] = w.loc[i-1][0] + dx * wb[2];
				w.loc[i][1] = w.loc[i-1][1] + dy * wb[2];
			}
		}

		++i;
	}
}

