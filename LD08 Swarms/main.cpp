//--------------------------------
//	Includes
//--------------------------------
#include <ptk.h>
#include "pSprite.h"
#include <vector>
#include <math.h>
#include "SmallFunc.h"

using namespace std;
//--------------------------------
//	Defines
//--------------------------------
#undef UNICODE
#define MOVE_PLAYER	10000.0f
#define MOVE_WORM	 9000.0f
#define WORM_DIST	 300.0f

#define FPS_PLAYER	 330000.0f
#define FPS_WORM	4410000.0f
#define FPS_BOOM	 110000.0f

#define SOUND_DECAY 990000.0f
#define HOLD_TIME	1000.0f

#define LEVEL_MAX	10

#define GAME_INTRO	1
#define GAME_READY	2
#define GAME_PLAY	3
#define GAME_WON	4
#define GAME_LOST	5

#define POW_MAG		100.0f

#define SP_PLAYER	0
#define SP_END		1
#define SP_WORM		2

//--------------------------------
//	Globals
//--------------------------------
KWindow*		g_window;
KTrueText*		g_font;
uXY				g_mapSize;
pXY				g_Trans;
pMoveable		g_Player;
vector<KGraphic*>	g_Images;
vector<pSound>		g_Sounds;
vector<pMoveable>	g_Worms;
vector<pStatic>		g_Objects;

float g_soundAge;
float g_Difficulty;
int g_Flower;
int g_Grenade;
int g_level;
int g_GameState;
unsigned long g_Delta;
unsigned long g_holdTime;
//--------------------------------
//	Predeclaration
//--------------------------------
void startGame(char* _file);
void winGame();
void loseGame();
void playGame();
void menuScreen();

int loadMap(char* _file);
void gameStep();
void update();
void render();
bool gameEventHandler(KEvent* _eventPtr);
bool menuEventHandler(KEvent* _eventPtr);

pSprite g_SPplayer;
pSprite g_SPworm;
pSprite g_SPboom;

pXY bCalcCOM(unsigned int c);
pXY bCalcSoundCOM(unsigned int c);
pXY bCalcAvoid(unsigned int c);
pXY bCalcFollow(unsigned int c);


//========================================================================
//	WINDOWS MAIN FUNCTION
//========================================================================
int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nShowCmd )
{
	g_Difficulty = 1.0f;
	g_GameState = GAME_INTRO;
	unsigned long _oldTime = KMiscTools::getMilliseconds();
	unsigned long _newTime = 0;
	//-------------------------------
	//	Create Window
	//-------------------------------
    g_window = KPTK::createKWindow(K_DIRECTX);
	g_window->createGameWindow(800,600,32,true,"Dune: Worms");
	g_window->setClearColor(0,0,1,0);

	KTrueText _font("Chiller.ttf");
	g_font = &_font;

	//-----------------------------------
	//	Main Loop
	//-----------------------------------
	do{
		_newTime = KMiscTools::getMilliseconds();
		g_Delta = (_oldTime - _newTime) / 100000;
		_oldTime = _newTime;

		switch(g_GameState)
		{
		case GAME_INTRO:
			menuScreen();
			break;
		case GAME_WON:
			winGame();
			break;
		case GAME_LOST:
			loseGame();
			break;
		case GAME_READY:
		case GAME_PLAY:
			playGame();
			break;
		};
	} while(g_window->isQuit() == false);	
    return 0;
}
//========================================================================
//	Load Map Information
//========================================================================
int loadMap(char* _file)
{
	srand(KMiscTools::getMilliseconds());
	char _buffer[50];
	KIniReader* _iniReader = new KIniReader;
	if(_iniReader->setIni(_file))
	{
		//--------------------------------
		//	Load Header
		//--------------------------------
		_iniReader->getString("MAPINFO","start", _buffer , 50);
		int x,y;
		sscanf(_buffer,"%i %i",&x,&y);
		g_Player.m_pos[0] = (float)x;
		g_Player.m_pos[1] = (float)y;

		_iniReader->getString("MAPINFO","size", _buffer , 50);
		sscanf(_buffer,"%i %i",&(g_mapSize.x),&(g_mapSize.y));

		g_Grenade = (int)_iniReader->getValue("MAPINFO","grenade", 0);

		//--------------------------------
		//	Load Worms
		//--------------------------------
		char _cat[15];
		int c = 1;
		while(c > 0)
		{
			sprintf(_cat,"%i_WORM",c);
			int _count = (int)_iniReader->getValue(_cat,"count", 0);
			if(_count)
			{
				int _px,_py;
				_iniReader->getString(_cat,"pos", _buffer , 50);
				sscanf(_buffer,"%i %i",&_px,&_py);

				int _sx,_sy;
				_iniReader->getString(_cat,"size", _buffer , 50);
				sscanf(_buffer,"%i %i",&_sx,&_sy);
				//-------------------------
				//	Spawn X worms at point
				//-------------------------
				while(_count)
				{
					pMoveable _temp = {0};
					_temp.m_pos[0] = (float)((rand() % _sx)-(_sx/2) + _px);
					_temp.m_pos[1] = (float)((rand() % _sy)-(_sy/2) + _py);
					g_Worms.push_back(_temp);
					--_count;
				}
				++c;
			}
			else
			{
				c = 0;
			}
		}
		//-------------------------
		//	Load Items
		//-------------------------
		c = 1;
		while(c > 0)
		{
			sprintf(_cat,"%i_ITEM",c);
			if(_iniReader->getString(_cat,"pic", _buffer , 50))
			{
				//---------------------------------
				//	Get Object Data
				//---------------------------------
				pStatic _temp = {0};
				KGraphic* _kPtr = KPTK::createKGraphic();
				g_Images.push_back(_kPtr);
				_kPtr->loadPicture(_buffer,true,true);
				_temp.m_pic = _kPtr;
				_temp.m_size[0] = _kPtr->getWidth();
				_temp.m_size[1] = _kPtr->getHeight();
				_temp.m_walk = (_iniReader->getValue(_cat,"walk", 0) == 0);
				_temp.m_pickup = (int)_iniReader->getValue(_cat,"drop", 0);


				//---------------------------------
				//	Get Instances of Object
				//---------------------------------
				char _objStr[10];
				int i = 1;
				sprintf(_objStr,"obj%i",i);
				while(_iniReader->getString(_cat,_objStr, _buffer , 50))
				{
					int x,y;
					sscanf(_buffer,"%i %i",&x,&y);
					_temp.m_pos[0] = (float)x;
					_temp.m_pos[1] = (float)y;
					g_Objects.push_back(_temp);
					++i;
					sprintf(_objStr,"obj%i",i);
				}	
				if(_temp.m_pickup == 5)
					g_Flower = (i-1);
				++c;
			}
			else
			{
				c = 0;
			}
		}
		//-------------------------------
		//	Return Success
		//-------------------------------
		return 1;
	}
	else
	{
		--g_level;
		if(g_level < 1)
			return 0;
		char _buff[50];

		sprintf(_buff,"map%i.ini",g_level);
		return loadMap(_buff);
	}
}

//========================================================================
//	Step of Game Logic
//========================================================================
void gameStep()
{
	//-------------------------
	//	Get Direction Vector
	//-------------------------
	Norm(g_Player.m_dir[0],g_Player.m_dir[1]);
	/*
	if(!(KInput::getRightButtonState()))
	{
		g_Player.m_dir[0] = (float)(KInput::getMouseX()) + g_Trans.x;
		g_Player.m_dir[1] = (float)(KInput::getMouseY()) + g_Trans.y;

		float _dx = g_Player.m_pos[0] - g_Player.m_dir[0];
		float _dy = g_Player.m_pos[1] - g_Player.m_dir[1];
		float _dmag = Magnitude(_dx,_dy);

		g_Player.m_dir[0] = _dx / _dmag;
		g_Player.m_dir[1] = _dy / _dmag;
	}
	else
	{
		g_Player.m_dir[0] = 0;
		g_Player.m_dir[1] = 0;
	}*/

	//-------------------------
	//	Update Sounds
	//-------------------------
	for(unsigned int c = 0; c < g_Sounds.size(); ++c)
	{
		pSound& _obj = g_Sounds[c];

		//-------------------------
		//	Delete Dead Sound
		//-------------------------
		if(_obj.m_age < 0.0f)
		{
			g_Sounds.erase(g_Sounds.begin() + c);
		}
		else
		{
			_obj.m_age -= g_Delta / SOUND_DECAY * g_Difficulty;
			if(_obj.m_frame < 50.0f)
				_obj.m_frame = min(5.0f,(_obj.m_frame + g_Delta / FPS_BOOM));
		}
	}
	//-------------------------
	//	Calculate Worms
	//-------------------------
	for(unsigned int c = 0; c < g_Worms.size(); ++c)
	{
		pMoveable& _obj = g_Worms[c];
		//-----------------------------
		//	Calculate Rules
		//-----------------------------
		pXY _com = bCalcCOM(c);
		pXY _avoid = bCalcAvoid(c);
		pXY _follow = bCalcFollow(c);
		pXY _sound = bCalcSoundCOM(c);
		
		//-----------------------------
		// Normalise
		//-----------------------------
		Norm(   _com.x,   _com.y,0.00001f);
		Norm( _avoid.x, _avoid.y,1000.0f);
		Norm(_follow.x,_follow.y,0.00001f);
		Norm( _sound.x, _sound.y,100.0f);

		pXY _final = {
			_com.x + _avoid.x + _follow.x + _sound.x,
			_com.y + _avoid.y + _follow.y + _sound.y};
		
		Norm(_final.x, _final.y);

		_obj.m_dir[0] += (_final.x) * g_Delta / 300000 * g_Difficulty;
		_obj.m_dir[1] += (_final.y) * g_Delta / 300000 * g_Difficulty;
		Norm(_obj.m_dir[0], _obj.m_dir[1]);
	}
}
//========================================================================
//	Update Animations and Graphics Vari
//========================================================================
void update()
{
	//--------------------------------
	//	Move Player
	//--------------------------------
	if(Magnitude(g_Player.m_pos[0],g_Player.m_pos[1]) > 0.1f)
	{
		bool flag = true;
		bool _safe = false;
		pMoveable& _obj = g_Player;
		
		//---------------------------------------
		//	Check Move
		//---------------------------------------
		float x = _obj.m_pos[0] - _obj.m_dir[0] * (g_Delta / MOVE_PLAYER * g_Difficulty);
		float y = _obj.m_pos[1] - _obj.m_dir[1] * (g_Delta / MOVE_PLAYER * g_Difficulty);
		x = min(max(0,x),g_mapSize.x);
		y = min(max(0,y),g_mapSize.y);
		for(unsigned int c = 0; c < g_Objects.size(); ++c)
		{
			if( ((g_Objects[c].m_size[0] * 0.6f) > abs(g_Objects[c].m_pos[0] - x)) && 
				((g_Objects[c].m_size[1] * 0.6f) > abs(g_Objects[c].m_pos[1] - y)) )
			{
				if(g_Objects[c].m_walk)
				{
					_safe = true;
				}
				else
				{
					flag = false;
				}
				switch(g_Objects[c].m_pickup)
				{
				case 1: // END
					if(!g_Flower)
					{
						g_GameState = GAME_WON;
						return;
					}
					break;
				case 5:	// Flower
					--g_Flower;
					g_Objects.erase(g_Objects.begin() + c);
					break;
				case 10: // Grenades
					++g_Grenade;
					g_Objects.erase(g_Objects.begin() + c);
				};
			}
		}

		//---------------------------------------
		//	Make Move
		//---------------------------------------
		if(flag)
		{
			_obj.m_pos[0] = x;
			_obj.m_pos[1] = y;
			if(!_safe)
			{
				pSound _temp ={0};
				_temp.m_age = 0.5f;
				_temp.m_pow = 1.0f;
				_temp.m_frame = 999.0f;
				_temp.x = x;
				_temp.y = y;

				g_Sounds.push_back(_temp);
			}
		}
	}
	//---------------------------------------
	//	Update Animation
	//---------------------------------------
	g_SPplayer.nextFrame(g_Delta / FPS_PLAYER);

	//---------------------------------------
	//	Age Sound
	//---------------------------------------
	g_soundAge -= (g_Delta / SOUND_DECAY * g_Difficulty);

	//--------------------------------
	//	Move Worms
	//--------------------------------
	for(unsigned int c = 0; c < g_Worms.size(); ++c)
	{
		bool flag = true;
		bool _eat = false;
		pMoveable& _obj = g_Worms[c];
		float x = _obj.m_pos[0] + _obj.m_dir[0] * (g_Delta / MOVE_WORM* g_Difficulty);
		float y = _obj.m_pos[1] + _obj.m_dir[1] * (g_Delta / MOVE_WORM* g_Difficulty);
		x = min(max(0,x),g_mapSize.x);
		y = min(max(0,y),g_mapSize.y);
		for(unsigned int c = 0; c < g_Objects.size(); ++c)
		{

			flag = flag 
				&& (
				((g_Objects[c].m_size[0] * 0.6f) < abs(g_Objects[c].m_pos[0] - x)) ||
				((g_Objects[c].m_size[1] * 0.6f) < abs(g_Objects[c].m_pos[1] - y)) ||
				(g_Objects[c].m_pickup > 0));

			_eat = _eat || (((25) > abs(g_Player.m_pos[0] - x)) && ((25) > abs(g_Player.m_pos[1] - y)));
		}
		if(flag)
		{
			_obj.m_pos[0] = x;
			_obj.m_pos[1] = y;
			if(_eat)
				g_GameState = GAME_LOST;
		}
		g_SPworm.nextFrame(g_Delta / FPS_WORM);
	}	
}
//========================================================================
//	Render
//========================================================================
void render()
{	
		//---------------------------------------
		//	Update Map Tracking
		//---------------------------------------
		g_Trans.x = min(max(-5.0f,g_Player.m_pos[0] - 400), g_mapSize.x - 755);
		g_Trans.y = min(max(-5.0f,g_Player.m_pos[1] - 300), g_mapSize.y - 525);
		//---------------------------------------------
		//	Backdrop
		//---------------------------------------------
		g_window->setDefaultWorldView();		//resets the transformation matrix
		g_Images[0]->blitRect(0,0,800,600,0,0); 

		g_window->setWorldView(
			-g_Trans.x,g_Trans.y,
			0.0f,1.0f,false);

		//--------------------------------
		//	Draw Objects
		//--------------------------------
		for(int c = (g_Objects.size() - 1); c >= 0; --c)
		{			
			short _tx = (short)(g_Objects[c].m_pos[0] - (g_Objects[c].m_size[0] / 2.0f));
			short _ty = (short)(g_Objects[c].m_pos[1] - (g_Objects[c].m_size[1] / 2.0f));

			if((!g_Flower) || (g_Objects[c].m_pickup != 1))
				g_Objects[c].m_pic->blitAlphaRect(0,0,g_Objects[c].m_size[0],g_Objects[c].m_size[1],
				_tx,_ty, 
				false, false);
		}
		//--------------------------------
		//	Draw Explosion
		//--------------------------------
		for(unsigned int c = 0; c < g_Sounds.size(); ++c)
		{
			pSound& _obj = g_Sounds[c];

			g_SPboom.setCycle(0,_obj.m_frame);
			g_SPboom.draw(_obj.x,_obj.y);
		}
		//--------------------------------
		//	Draw Worms
		//--------------------------------
		for(unsigned int c = 0; c < g_Worms.size(); ++c)
		{
			if(abs(g_Worms[c].m_dir[0]) > 0.8f)
				if(g_Worms[c].m_dir[0] > 0.0f)
					g_SPworm.setCycle(1);
				else
					g_SPworm.setCycle(3);
			else if(abs(g_Worms[c].m_dir[1]) > 0.8f)
				if(g_Worms[c].m_dir[1] > 0.0f)
					g_SPworm.setCycle(2);
				else
					g_SPworm.setCycle(0);
			else if(g_Worms[c].m_dir[0] > 0.0f)
				if(g_Worms[c].m_dir[1] > 0.0f)
					g_SPworm.setCycle(5);
				else
					g_SPworm.setCycle(4);
			else if(g_Worms[c].m_dir[1] > 0.0f)
				g_SPworm.setCycle(0);
			else
				g_SPworm.setCycle(0);


			g_SPworm.draw(g_Worms[c].m_pos[0],g_Worms[c].m_pos[1]);
		}
		//--------------------------------
		//	Draw Player
		//--------------------------------
		int _cycle = 0;
		if(abs(g_Player.m_dir[0]) > 0.7f)
			if(g_Player.m_dir[0] > 0.0f)
				g_SPplayer.setCycle(3);
			else
				g_SPplayer.setCycle(1);
		else if(abs(g_Player.m_dir[1]) > 0.7f)
			if(g_Player.m_dir[1] > 0.0f)
				g_SPplayer.setCycle(0);
			else
				g_SPplayer.setCycle(2);


		g_SPplayer.draw(g_Player.m_pos[0],g_Player.m_pos[1]);
		//--------------------------------
		//	Draw Foreground
		//--------------------------------
		g_window->setWorldView(0,0,0,1.0f,false);
		g_Images[1]->blitAlphaRect(0,0,800,600,0,0); 
		//--------------------------------
		//	Draw Flowers or Drop
		//--------------------------------
		if(g_Flower)
		{
			for(int c = 0; c < g_Flower; ++c)
			{
				g_Images[2]->blitAlphaRect(0,0,32,32,150+(c*40),525);
			}
		}
		else
		{
			g_Images[3]->blitAlphaRect(0,0,352,64,150,470);
		}
		//--------------------------------
		//	Draw Grenade
		//--------------------------------
		if(g_Grenade)
		{
			// Banner
			if(KInput::getLeftButtonState() && (g_holdTime != 0))
			{
				float _per = float(KMiscTools::getMilliseconds() - g_holdTime) / (HOLD_TIME / g_Difficulty);
				_per = min(_per,1.5f);
				g_Images[4]->blitAlphaRect(floorf(_per)*271,0,floorf(_per + 1.0f)*271,80,512,8);
				if(_per < 1.0f)
					g_Images[4]->drawRect(512,80,512+(271*_per),90,1,0,0,1);
			}

			// Icon
			for(int c = 0; c < g_Grenade; ++c)
			{
				g_Images[5]->blitAlphaRect(0,0,32,32,150+(c*40),563);
			}
		}
		//--------------------------------
		//	Draw Darker Screen
		//--------------------------------
		if(g_GameState == GAME_READY)
		{
			g_Images[0]->drawRect(0,0,800,600,0,0,0,0.5f);

			g_font->setHeightPix(101);
			g_font->setColor(1,1,1,1);
			g_font->drawStringCentered("Click 2 Start",200,600,400);

			g_font->setHeightPix(100);
			g_font->setColor(0,0,0,1);
			g_font->drawStringCentered("Click 2 Start",200,600,400);
		}
		else
		{
			g_font->setHeightPix(30);
			g_font->setColor(0,0,0,1);
			char _buff[50];
			sprintf(_buff,"Level: %02i",g_level);
			g_font->drawStringFromLeft(_buff,20,10);
		}
		//---------------------------------------------
		g_window->flipBackBuffer();			//display the drawing
}
//========================================================================
//	Process Events
//========================================================================
bool gameEventHandler(KEvent* _eventPtr)
{
	switch(_eventPtr->type)
	{
	case K_EVENT_MOUSEUP:
		//-------------------------
		//	RELEASE MOUSE
		//-------------------------
		if(g_GameState == GAME_READY)
		{
			g_GameState = GAME_PLAY;
			return true;
		}
		if(_eventPtr->buttonIndex == K_LBUTTON)
		{
			if((g_Grenade) && ((KMiscTools::getMilliseconds() - g_holdTime) > (HOLD_TIME/g_Difficulty)))
			{
				g_Grenade -= 1;
				pSound _sound = {0};
				_sound.x = g_Trans.x + _eventPtr->mouseX;
				_sound.y = g_Trans.y + _eventPtr->mouseY;
				_sound.m_age = 10.0f; //(max(0.0f,400.0f - Magnitude(_sound.x - g_Player.m_pos[0],_sound.y - g_Player.m_pos[1]))) / 25.0f;
				_sound.m_frame = 0;
				_sound.m_pow = 50.0f; //min(((KMiscTools::getMilliseconds() - g_holdTime) / POW_MAG), 25.0f);

				g_Sounds.push_back(_sound);				
			}
			g_holdTime = 0;
		}
		break;
	case K_EVENT_MOUSEDOWN:
		//-------------------------
		//	LEFT MOUSE
		//-------------------------
		if(_eventPtr->buttonIndex == K_LBUTTON)
		{
			g_holdTime = KMiscTools::getMilliseconds();
		}
		break;
	case K_EVENT_KEYUP:
		switch(_eventPtr->ptkKey)
		{
		case K_VK_ESCAPE:
			g_GameState = GAME_INTRO;
			break;
		case K_VK_W:
			g_Player.m_dir[1] = min(0.0f,g_Player.m_dir[1]);
			break;
		case K_VK_S:
			g_Player.m_dir[1] = max(0.0f,g_Player.m_dir[1]);
			break;
		case K_VK_A:
			g_Player.m_dir[0] = min(0.0f,g_Player.m_dir[0]);
			break;
		case K_VK_D:
			g_Player.m_dir[0] = max(0.0f,g_Player.m_dir[0]);
			break;
		};
		break;
	case K_EVENT_KEYDOWN:
		switch(_eventPtr->ptkKey)
		{
		case K_VK_W:
			g_Player.m_dir[1] = 1.0f;
			break;
		case K_VK_S:
			g_Player.m_dir[1] = -1.0f;
			break;
		case K_VK_A:
			g_Player.m_dir[0] = 1.0f;
			break;
		case K_VK_D:
			g_Player.m_dir[0] = -1.0f;
			break;
		};
		break;
	};
	return true;
}
//========================================================================
//	Handels Events while in Menu
//========================================================================
bool menuEventHandler(KEvent* _eventPtr)
{
	if(_eventPtr->type == K_EVENT_KEYUP)
	{
		switch(_eventPtr->ptkKey)
		{
		case K_VK_RETURN:
			g_GameState = -1;
			break;
		case K_VK_LEFT:
			g_Difficulty = max(0.5f,g_Difficulty - 0.05f);
			break;
		case K_VK_RIGHT:
			g_Difficulty = min(2.0f,g_Difficulty + 0.05f);
			break;
		case K_VK_ESCAPE:
			g_window->terminate();
			break;
		};
	}
	return true;
}
//========================================================================
//	Calculate COM as percieved by WORM
//========================================================================
pXY bCalcCOM(unsigned int c)
{
	pMoveable& _obj = g_Worms[c];
	pXY _point = {0};
	int _count = 0;

	for(unsigned int i = 0; i < g_Worms.size(); ++i)
	{	
		//----------------------------------
		//	Limit Sphere of perception
		//----------------------------------
		pMoveable& _other = g_Worms[i];
		float _dist = Magnitude(_obj.m_pos[0] - _other.m_pos[0],_obj.m_pos[1] - _other.m_pos[1]);

		if((c != i) && (_dist < WORM_DIST))
		{
			_point.x += _other.m_pos[0];
			_point.y += _other.m_pos[1];
			++_count;
		}
	}

	pXY _vector = {0,0};
	if(_count)
	{
		_point.x /= float(_count);
		_point.y /= float(_count);

		_vector.x = _point.x - _obj.m_pos[0];
		_vector.y = _point.y - _obj.m_pos[1];
	}

	return _vector;
}
//========================================================================
//	Calculate Avoidence
//========================================================================
pXY bCalcAvoid(unsigned int c)
{
	pMoveable& _obj = g_Worms[c];
	pXY _vector = {0,0};	
	int _count = 0;

	//--------------------------------------------------------
	//	Avoid Fellow Worms
	//--------------------------------------------------------
	for(unsigned int i = 0; i < g_Worms.size(); ++i)
	{	
		//----------------------------------
		//	Limit Sphere of perception
		//----------------------------------
		pMoveable& _other = g_Worms[i];
		float _dx = _other.m_pos[0] - _obj.m_pos[0];
		float _dy = _other.m_pos[1] - _obj.m_pos[1];
		float _dist = Magnitude(_dx,_dy);

		if((c != i) && (_dist < (WORM_DIST * 0.2f)))
		{
			_vector.x -= _dx;
			_vector.y -= _dy;
		}
	}

	//--------------------------------------------------------
	//	Avoid Objects
	//--------------------------------------------------------
	pXY _Objvector = {0,0};	
	for(unsigned int i = 0; i < g_Objects.size(); ++i)
	{	
		//----------------------------------
		//	Limit Sphere of perception
		//----------------------------------
		pStatic& _other = g_Objects[i];

		//--------------------------------
		//	Get Distance to nearest edge
		//--------------------------------
		float _dx = _other.m_pos[0] - _obj.m_pos[0];
		float _dy = _other.m_pos[1] - _obj.m_pos[1];

		//--------------------------------
		//	Avoid
		//--------------------------------
		if(_other.m_pickup == 0)
		if(((abs(_dx)) < (_other.m_size[0] * 0.65f)) && ((abs(_dy)) < (_other.m_size[1] * 0.65f + 32)))
		{
			if(abs(_dx) < abs(_dy))
			{
				if(_dx > 0)		_Objvector.x -= 0.8f;
				else			_Objvector.x += 0.8f;
				if(_dy > 0)		_Objvector.y -= 0.2f;
				else			_Objvector.y += 0.2f;
			}
			else
			{
				if(_dx > 0)		_Objvector.x -= 0.2f;
				else			_Objvector.x += 0.2f;
				if(_dy > 0)		_Objvector.y -= 0.8f;
				else			_Objvector.y += 0.8f;
			}
		}
	}
	//--------------------------------
	//	Weight Importance to Blocks
	//--------------------------------
	Norm(_vector.x,_vector.y);
	Norm(_Objvector.x,_Objvector.y);
	_vector.x += _Objvector.x;
	_vector.y += _Objvector.y;
	return _vector;
}
//========================================================================
//	Calculate Matching Vector
//========================================================================
pXY bCalcFollow(unsigned int c)
{
	pMoveable& _obj = g_Worms[c];
	pXY _vector = {0,0};	
	int _count = 0;

	for(unsigned int i = 0; i < g_Worms.size(); ++i)
	{	
		//----------------------------------
		//	Limit Sphere of perception
		//----------------------------------
		pMoveable& _other = g_Worms[i];
		float _dx = _other.m_pos[0] - _obj.m_pos[0];
		float _dy = _other.m_pos[1] - _obj.m_pos[1];
		float _dist = Magnitude(_dx,_dy);

		if((c != i) && (_dist < (WORM_DIST)))
		{
			_vector.x += _dx;
			_vector.y += _dy;
		}
	}
	return _vector;
}
//========================================================================
//	Calculate hearing Vector for Sound
//========================================================================
pXY bCalcSoundCOM(unsigned int c)
{
	pMoveable& _obj = g_Worms[c];
	pXY _point = {0};
	int _count = 0;

	for(unsigned int i = 0; i < g_Sounds.size(); ++i)
	{	
		//----------------------------------
		//	Limit Sphere of perception
		//----------------------------------
		pSound& _other = g_Sounds[i];
		float _dist = Magnitude(_obj.m_pos[0] - _other.x,_obj.m_pos[1] - _other.y);

		if((c != i) && (_dist < WORM_DIST))
		{
			//-------------------------------------
			//	Override Sound for Grenades
			//	QUICK HACK
			//-------------------------------------
			if(_other.m_pow > 25.0f)
			{
				pXY _vector = {
					_other.x - _obj.m_pos[0],
					_other.y - _obj.m_pos[1]};
				return _vector;
			}
			_point.x += _other.x;
			_point.y += _other.y;
			++_count;
		}
	}

	pXY _vector = {0,0};
	if(_count)
	{
		_point.x /= float(_count);
		_point.y /= float(_count);

		_vector.x = _point.x - _obj.m_pos[0];
		_vector.y = _point.y - _obj.m_pos[1];
	}

	return _vector;
}
//========================================================================
//	Start a Game :: THIS IS NOTE MENU STATE
//========================================================================
void startGame(char* _file)
{
	//-------------------------------
	//	Sprite Initialisation
	//-------------------------------
	{
		// Player
		if(g_SPplayer.empty())
			g_SPplayer.createSprite(64,64,"runner.png");
		g_SPplayer.setCycle(1,0);

		// Worms
		if(g_SPworm.empty())
			g_SPworm.createSprite(32,32,"worm.png");	

		// Explosion Sprite
		if(g_SPboom.empty())
			g_SPboom.createSprite(64,64,"boom.png");

		// Background
		KGraphic* _backPtr = KPTK::createKGraphic();
		g_Images.push_back(_backPtr);
		_backPtr->loadPicture("back.png", false, true);

		// Overlay
		KGraphic* _overPtr = KPTK::createKGraphic();
		g_Images.push_back(_overPtr);
		_overPtr->loadPicture("overlay.png", false, true);

		// Flower Icon
		KGraphic* _fico = KPTK::createKGraphic();
		g_Images.push_back(_fico);
		_fico->loadPicture("flowico.png", false, true);

		// Ship Icon
		KGraphic* _sico = KPTK::createKGraphic();
		g_Images.push_back(_sico);
		_sico->loadPicture("shipico.png", false, true);

		// Grenade BANNER
		KGraphic* _gbanner = KPTK::createKGraphic();
		g_Images.push_back(_gbanner);
		_gbanner->loadPicture("grenade.png", false, true);

		// Grenade Icon
		KGraphic* _gico = KPTK::createKGraphic();
		g_Images.push_back(_gico);
		_gico->loadPicture("greico.png", false, true);
	}
	//-------------------------------
	//	Load Map
	//-------------------------------
	loadMap(_file);

	g_window->setPTKCallBack(&gameEventHandler);
	g_GameState = GAME_READY;
}
//========================================================================
//	Play Winning Screen
//========================================================================
void winGame()
{
	//---------------------------
	//	Clean Up After Last Game
	//---------------------------
	g_Objects.clear();
	g_Worms.clear();
	g_Sounds.clear();
	g_Images.clear();

	char _buff[50];
	g_level = min(g_level+1,LEVEL_MAX);
	sprintf(_buff,"map%i.ini",g_level);
	startGame(_buff);
}
//========================================================================
//	Play Dying Screen
//========================================================================
void loseGame()
{
	//---------------------------
	//	Clean Up After Last Game
	//---------------------------
	g_Objects.clear();
	g_Worms.clear();
	g_Sounds.clear();
	g_Images.clear();

	//-------------------------------
	//	Setup End Sprite
	//-------------------------------
	KGraphic* _eat = KPTK::createKGraphic();
	_eat->loadPicture("eat.png",true,false);
	float _frame = 0.0f;
	g_window->setDefaultWorldView();
	_eat->drawRect(0,0,800,600,0,0,0,0.5f);

	//-------------------------------
	//	Local Timer
	//-------------------------------
	unsigned long _oldTime = KMiscTools::getMilliseconds();
	unsigned long _newTime = 0;

	while(_frame < 5.0f)
	{
		//-------------------------
		//	Get Delta
		//-------------------------
		_newTime = KMiscTools::getMilliseconds();
		g_Delta = (_oldTime - _newTime) / 100000;
		_oldTime = _newTime;

		_frame += g_Delta / 1000000.0f;
		//-------------------------
		//	Draw
		//-------------------------
		_eat->blitRectFxF(floorf(_frame)*80,0,floorf(_frame+1)*80,60,200,150,0,5.0f,false,false);
		g_window->flipBackBuffer();			//display the drawing
	}

	char _buff[50];
	sprintf(_buff,"map%i.ini",g_level);
	startGame(_buff);
}
//========================================================================
//	Play Game
//========================================================================
void playGame()
{
	//-------------------------
	//	Time Elapse
	//-------------------------
	if(g_GameState == GAME_PLAY)
	{
		gameStep();						// Game Logic
		update();						// Update Animations
	}
	if((g_GameState == GAME_PLAY) || (g_GameState == GAME_READY))
	{
		render();						// Draw
		g_window->processEvents();		// Process Input
	}
}
//========================================================================
//	Display Menu
//========================================================================
void menuScreen()
{
	//-------------------------------
	//	Setup
	//-------------------------------
	char _buff[50];

	KGraphic* _back = KPTK::createKGraphic();
	_back->loadPicture("frontscreen.png",true,false);

	g_font->setHeightPix(32);
	g_font->setColor(0,0,0,1);
	g_level = 1;
	g_Difficulty = 1.0f;

	g_window->setPTKCallBack(&menuEventHandler);

	do
	{
		//-------------------------
		//	Draw
		//-------------------------
		g_window->setDefaultWorldView();
		_back->blitRect(0,0,800,600,0,0);

		g_font->drawStringCentered("Welcome to Arakis, the hell hole you will be harvesting spice plants in."		,40,760,280);
		g_font->drawStringCentered("Harvest all the plants in the area and then we will come pick you up."		,40,760,315);
		g_font->drawStringCentered("The Worms will be attract to anysound, your footsteps or the grenades."		,40,760,350);
		g_font->drawStringCentered("Walking on top of hard surfaces will give you a limited degree of protection.",40,760,385);

		int _temp = g_Difficulty * 100;
		sprintf(_buff,"<-- %03i%% -->",_temp);
		g_font->drawStringCentered("Speed",300,500,460);
		g_font->drawStringCentered(_buff,300,500,500);
		g_font->drawStringCentered("Press Return",300,500,540);

		g_window->flipBackBuffer();			//display the drawing
		g_window->processEvents();
	}while((g_GameState == GAME_INTRO) && (g_window->isQuit() == false));

	sprintf(_buff,"map%i.ini",g_level);
	startGame(_buff);
}