/********************************************************************
	created:	30:8:2009   12:28
	file path:	c:\Dev\LD_Caverns\Caverns\gameState.cpp
	author:		Claire Blackshaw
	
	purpose:	
*********************************************************************/

//////////////////////////////////////////////////////////////////////////
///	Game Includes
#include "gameState.h"
#include "CaveGener.h"
#include "GameClient.h"
#include <math.h>

static const float WALK_SPEED = 200.0f;
static const float SPRITE_OFFSETX = 290.0f;
static const float SPRITE_OFFSETY = 290.0f;
static const SDL_Rect MAP_AREA = { 30, 30, 550, 550};

static const SDL_Colour RED_TEXT_COLOUR = { 0xA0, 0x10, 0x10, 0xFF };
static const SDL_Colour WHITE_TEXT_COLOUR = { 0xFF, 0xFF, 0xFF, 0xFF };


//////////////////////////////////////////////////////////////////////////
///	[] gameState::gameState
gameState::gameState()
{
	m_ePhase = SP_DORMANT;

	m_pGUISurface	= NULL;
	m_pCaveSurface	= NULL;
	m_pShadowMap	= NULL;
	m_pShadowTiles	= NULL;
	m_pMonsterImage = NULL;
	m_pPlayerSprite = NULL;
	m_pRestartMsg		= NULL;
	m_pDeathMsg		= NULL;
	m_pDepthMsg		= NULL;
	m_pScoreMsg		= NULL;
	m_pConsoleMsg	= NULL;
	m_pInventory	= NULL;
	m_pMsgFont		= NULL;
	m_pDepthFont	= NULL;

	// Player Information
	m_playerFrame	= 0;
	m_playerFace	= DIR_NORTH;
	m_playerWalk	= NULL;
	m_playerX		= 0.0f;
	m_playerY		= 0.0f;
	m_SteamPower	= 1.0f;
	m_FuelReserve	= 1.0f;
	m_MonsterChance	= 2000;
	m_Kills			= NULL;
	memset(m_pItems, 0, sizeof(m_pItems));
	memset(m_pShadows, 0, sizeof(m_pShadows));

	// Map Information
	m_pCaveGener = NULL;
	m_caveVisited = NULL;
	m_caveDepth = 0;
	m_caveWidth = 50;
	m_caveHeight = 50;

	m_ConsoleTimer = 0;

	memset(m_caveItems, 0, sizeof(m_caveItems));

	m_bShadows = FALSE;
	m_bDead = FALSE;
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] gameState::~gameState
gameState::~gameState()
{
	if((m_ePhase > SP_DORMANT) && (m_ePhase < SP_CLOSED))
	{
		closeState();
	}
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] gameState::loadResources
bool gameState::loadResources()
{
	m_ePhase = SP_LOADING;

	// Surfaces
	m_pGUISurface	= bClient::LoadTexture("GUI.png", TRUE);
	m_pShadowTiles	= bClient::LoadTexture("shadow.png", TRUE);
	m_pPlayerSprite	= bClient::LoadTexture("playerSpr.png", TRUE);
	m_pInventory	= bClient::LoadTexture("inventory.png", TRUE);
	m_pMonsterImage = NULL;
	m_pDepthMsg		= NULL;
	m_pScoreMsg		= NULL;
	m_pConsoleMsg	= NULL;

	m_pMsgFont = bClient::LoadFont("5by5.ttf", 20);
	m_pDepthFont = bClient::LoadFont("5by5.ttf", 100);

	m_pShadowMap = SDL_CreateRGBSurface( SDL_SWSURFACE, SHADOW_SIZE_X*16, SHADOW_SIZE_Y*16, SCREEN_BPP, 0xFF000000, 0x00FF0000, 0x0000FF00, 0x000000FF);

	m_pDeathMsg = TTF_RenderText_Solid(m_pDepthFont, "GAME OVER", RED_TEXT_COLOUR);
	m_pRestartMsg = TTF_RenderText_Solid(m_pMsgFont, "Press Space to Try Again", WHITE_TEXT_COLOUR);

	m_ePhase = SP_LOADED;
	return TRUE;
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] gameState::startState
void gameState::startState()
{
	// Setup New Player Information
	setupNewPlayer("Player 1", 0);
	setupNewLevel(1);

	m_ePhase = SP_ACTIVE;
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] gameState::closeState
void gameState::closeState()
{
	FREE_SURF(m_pGUISurface);
	FREE_SURF(m_pCaveSurface);
	FREE_SURF(m_pShadowMap);
	FREE_SURF(m_pShadowTiles);
	FREE_SURF(m_pMonsterImage);
	FREE_SURF(m_pRestartMsg);
	FREE_SURF(m_pDeathMsg);
	FREE_SURF(m_pDepthMsg);
	FREE_SURF(m_pScoreMsg);
	FREE_SURF(m_pConsoleMsg);
	FREE_SURF(m_pInventory);

	FREE_FONT(m_pMsgFont);

	SAFE_FREE(m_caveVisited);
	SAFE_FREE(m_pCaveGener);

	m_ePhase = SP_CLOSED;
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] gameState::processEvent
void gameState::processEvent( SDL_Event& rEvent )
{
	if(rEvent.type == SDL_KEYDOWN)
	{
		switch (rEvent.key.keysym.sym)
		{
		case SDLK_UP: 
			m_playerFace = DIR_NORTH;
			m_playerWalk |= (1 << DIR_NORTH);
			break;
		case SDLK_DOWN: 
			m_playerFace = DIR_SOUTH;
			m_playerWalk |= (1 << DIR_SOUTH);
			break;
		case SDLK_LEFT: 
			m_playerFace = DIR_WEST;
			m_playerWalk |= (1 << DIR_WEST);
			break;
		case SDLK_RIGHT: 
			m_playerFace = DIR_EAST;
			m_playerWalk |= (1 << DIR_EAST);
			break;
		}
	}
	else if(rEvent.type == SDL_KEYUP)
	{
		switch (rEvent.key.keysym.sym)
		{
		case SDLK_UP:
			m_playerWalk &= ~(1 << DIR_NORTH);
			break;
		case SDLK_DOWN: 
			m_playerWalk &= ~(1 << DIR_SOUTH);
			break;
		case SDLK_LEFT: 
			m_playerWalk &= ~(1 << DIR_WEST);
			break;
		case SDLK_RIGHT: 
			m_playerWalk &= ~(1 << DIR_EAST);
			break;
		case SDLK_SPACE:
			setupNewPlayer("Player", 0);
			setupNewLevel(1);
			break;
		case SDLK_s:
			// m_bShadows = !m_bShadows;
			break;
		}

		if(m_playerWalk & (1 << DIR_NORTH))
		{
			m_playerFace = DIR_NORTH;
		}
		else if(m_playerWalk & (1 << DIR_SOUTH))
		{
			m_playerFace = DIR_SOUTH;
		}
		else if(m_playerWalk & (1 << DIR_WEST))
		{
			m_playerFace = DIR_WEST;
		}
		else if(m_playerWalk & (1 << DIR_EAST))
		{
			m_playerFace = DIR_EAST;
		}
	}
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] gameState::updateLogic
void gameState::updateLogic()
{
	if(m_bDead == TRUE)
	{
		return;
	}

	if(m_FuelReserve <= 0.0f)
	{
		m_bDead = TRUE;
		PrintMsg("Ran out of Fuel for your Lamp... eaten by a Grue!");
		return;
	}

	if(m_SteamPower <= 0.0f)
	{
		m_bDead = TRUE;
		PrintMsg("The Cave monster stood on you... cave painting.");
		return;
	}

	float dSec = bClient::GetClient()->deltaSeconds();
	m_FuelReserve -= dSec * 0.02f;
	m_ConsoleTimer -= dSec;

	// Player Walk Cycle
	if(m_playerWalk > NULL)
	{
		float newX = m_playerX;
		float newY = m_playerY;

		if(m_playerWalk & (1 << DIR_NORTH))		{ newY = m_playerY - (dSec * WALK_SPEED); }
		if(m_playerWalk & (1 << DIR_SOUTH))		{ newY = m_playerY + (dSec * WALK_SPEED); }
		if(m_playerWalk & (1 << DIR_EAST))		{ newX = m_playerX + (dSec * WALK_SPEED); }
		if(m_playerWalk & (1 << DIR_WEST))		{ newX = m_playerX - (dSec * WALK_SPEED); }

		if(m_pCaveGener->GetPoint((UINT)(newX/16.0f), (UINT)(newY/16.0f)) < CAVE_DIRT)
		{
			m_playerX = newX;
			m_playerY = newY;

			// Random Encounter
			if((rand() % m_MonsterChance) < m_caveDepth)
			{
				m_SteamPower -= ((rand() % 13) + (rand() % 13)) * 0.01f;
				m_MonsterChance = 2000;
				++m_Kills;

				PrintMsg("Killed a cave monster");
				UpdateKillCount();
			}
			else
			{
				--m_MonsterChance;
			}

			UINT posX = (UINT)(m_playerX / 16.0f);
			UINT posY = (UINT)(m_playerY / 16.0f);

			// Check Map items
			for(UINT iItem = 0; iItem < MAX_MAP_ITEMS; ++iItem)
			{
				if(m_caveItems[iItem].m_type != OBJ_NOT_IN_USE)
				{
					if( (m_caveItems[iItem].x == posX) &&
						(m_caveItems[iItem].y == posY))
					{
						if(m_caveItems[iItem].m_type == OBJ_EXIT)
						{
							// Next Level
							setupNewLevel(m_caveDepth+1);
							PrintMsg("Descending Deeper");
						}
						else if(m_caveItems[iItem].m_type == OBJ_ITEM)
						{
							// Items
							switch(m_caveItems[iItem].m_itemType)
							{
							case ITEM_GAS:
								if(m_FuelReserve < 0.95f)
								{
									PrintMsg("Found more fuel");
									m_FuelReserve = MIN(1.0f, m_FuelReserve + 0.2f);
									m_caveItems[iItem].m_type = OBJ_NOT_IN_USE;
								}
								break;
							case ITEM_STEAM:
								if(m_SteamPower < 0.95f)
								{
									PrintMsg("Found more steam tanks");
									m_SteamPower = MIN(1.0f, m_SteamPower + 0.2f);
									m_caveItems[iItem].m_type = OBJ_NOT_IN_USE;
								}
								break;
							}
						}
					}
				}
			}

			/*
			UpdateShadows();
			RenderShadows();
			/**/
		}

		m_playerFrame += dSec * 20;
		if(m_playerFrame > 4.0f) { m_playerFrame -= 4.0f; }
	}

}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] gameState::renderFrame
void gameState::renderFrame()
{
	SDL_Rect playSprite = {0,0, 16,16};
	SDL_Rect tileArea =	{ 0, 0, MAP_AREA.w, MAP_AREA.h };
	SDL_Rect screenArea = { 0,0,SCREEN_WIDTH, SCREEN_HEIGHT};
	SDL_Rect spriteArea = 
	{
		(UINT(m_playerFrame )*16),(m_playerFace*16),
		16,16
	};

	CalculateScroll(tileArea, playSprite);

	// Draw Ground
	SDL_FillRect(bClient::GetClient()->getScreen(), &screenArea, 0x291808);

	// Draw Walls
	bClient::BlitSurface(m_pCaveSurface, NULL, MAP_AREA.x, MAP_AREA.y, &tileArea);

	// Draw Map Objects
	RenderMapObjects(tileArea);

	// Draw Shadows
	if(m_bShadows == TRUE)
	{
		bClient::BlitSurface(m_pShadowMap, NULL, 0 - ((tileArea.x) % 16) - 0, 0 - ((tileArea.y) % 16)  - 0);
	}

	// Draw Player
	bClient::BlitSurface(m_pPlayerSprite, NULL, playSprite.x, playSprite.y, &spriteArea);

	// Draw Bars
	SDL_Rect steamArea = {607, 548-(UINT)(275*m_SteamPower),  36, (UINT)(275*m_SteamPower)};
	SDL_Rect gasArea =   {738, 546-(UINT)(271*m_FuelReserve), 34, (UINT)(271*m_FuelReserve)};
	SDL_FillRect(bClient::GetClient()->getScreen(), &steamArea, 0x8800FFFF);
	SDL_FillRect(bClient::GetClient()->getScreen(), &gasArea, 0x88FF0000);

	// Draw GUI
	bClient::BlitSurface(m_pGUISurface, NULL);

	// Draw Stats
	if(m_ConsoleTimer > 2.0f)
	{
		bClient::BlitSurface(m_pConsoleMsg, NULL, MAP_AREA.x+10, MAP_AREA.y+3);
	}
	else if(m_ConsoleTimer > 0.0f)
	{
		SDL_Rect clipRect = 
		{ 
			0, 
			m_pConsoleMsg->h - (int)(m_ConsoleTimer * m_pConsoleMsg->h * 0.5f), 
			m_pConsoleMsg->w, 
			(int)(m_ConsoleTimer * m_pConsoleMsg->h * 0.5f), 
		};

		bClient::BlitSurface(m_pConsoleMsg, NULL, MAP_AREA.x+10, MAP_AREA.y+3, &clipRect);
	}

	// Draw Depth
	bClient::BlitSurface(m_pDepthMsg, NULL, 650, 50);	
	bClient::BlitSurface(m_pScoreMsg, NULL, 600,160);	

	if(m_bDead == TRUE)
	{
		bClient::BlitSurface(m_pDeathMsg, NULL, 50, 100);
			bClient::BlitSurface(m_pRestartMsg, NULL, 50, 550);
	}
	
}

//////////////////////////////////////////////////////////////////////////
///	[] gameState::CalculateScroll
void gameState::CalculateScroll( SDL_Rect &tileArea, SDL_Rect &playSprite)
{
	if(m_playerX < SPRITE_OFFSETX)
	{
		tileArea.x = 0;
	}
	else if(m_playerX > ((m_caveWidth*16)-SPRITE_OFFSETX))
	{
		tileArea.x = (m_caveWidth*16) - MAP_AREA.w;
	}
	else
	{
		tileArea.x = (UINT)(m_playerX - SPRITE_OFFSETX);
	}

	if(m_playerY < SPRITE_OFFSETY)
	{
		tileArea.y = 0;
	}
	else if(m_playerY > ((m_caveHeight*16)-SPRITE_OFFSETY))
	{
		tileArea.y = (m_caveHeight*16) - MAP_AREA.h;
	}
	else
	{
		tileArea.y = (UINT)(m_playerY - SPRITE_OFFSETY);
	}

	playSprite.x = (int)(m_playerX - tileArea.x + MAP_AREA.x - 8.0f);
	playSprite.y = (int)(m_playerY - tileArea.y + MAP_AREA.y - 8.0f);
}

//////////////////////////////////////////////////////////////////////////
///	[] gameState::RenderMapObjects
void gameState::RenderMapObjects( SDL_Rect &tileArea )
{
	for(UINT iItem = 0; iItem < MAX_MAP_ITEMS; ++iItem)
	{
		if( (m_caveItems[iItem].m_type == OBJ_NOT_IN_USE) || 
			((m_caveItems[iItem].x * 16.0f) < tileArea.x) ||
			((m_caveItems[iItem].y * 16.0f) < tileArea.y) ||
			((m_caveItems[iItem].x * 16.0f) > (tileArea.x + MAP_AREA.w)) ||
			((m_caveItems[iItem].y * 16.0f) > (tileArea.y + MAP_AREA.h)))
		{
			continue;
		}

		SDL_Rect objArea = 
		{
			int((m_caveItems[iItem].x * 16.0f) - tileArea.x + MAP_AREA.x), 
			int((m_caveItems[iItem].y * 16.0f) - tileArea.y + MAP_AREA.y), 
			16, 16
		};

		SDL_Rect objTile = {0,0,16, 16};

		switch (m_caveItems[iItem].m_type)
		{
		case OBJ_ENTRANCE:
			objTile.x = 16*3;
			objTile.y = 16*0;
			break;
		case OBJ_EXIT:
			objTile.x = 16*2;
			objTile.y = 16*0;
			break;
		case OBJ_ITEM:
			{
				switch (m_caveItems[iItem].m_itemType)
				{
				case ITEM_GAS:
					objTile.x = 16*0;
					objTile.y = 16*0;
					break;
				case ITEM_STEAM:
					objTile.x = 16*1;
					objTile.y = 16*0;
					break;
				};
			}
			break;
		case OBJ_MONSTER:
			{
				switch(m_caveItems[iItem].m_monType)
				{
				case MONST_GRUNTER:
					objTile.x = 16*0;
					objTile.y = 16*1;
					break;
				case MONST_TREX:
					objTile.x = 16*1;
					objTile.y = 16*1;
					break;
				};
			}
			break;
		}

		bClient::BlitSurface(m_pInventory, NULL, objArea.x, objArea.y, &objTile);
	}
}

//////////////////////////////////////////////////////////////////////////
///	[] gameState::setupNewPlayer
void gameState::setupNewPlayer( const char* pName, UINT nDifficulty )
{
	m_playerFrame	= 0;
	m_playerFace	= DIR_NORTH;
	m_playerWalk	= NULL;
	m_playerX		= 0.0f;
	m_playerY		= 0.0f;
	m_SteamPower	= 1.0f;
	m_FuelReserve	= 1.0f;
	m_MonsterChance	= 2000;
	m_Kills			= NULL;
	m_bDead			= FALSE;
	memset(m_pItems, 0, sizeof(m_pItems));

	PrintMsg("Welcome Brave Spelunker! How Deep can you go?");

	UpdateKillCount();

}

//////////////////////////////////////////////////////////////////////////
///	[] gameState::setupNewLevel
void gameState::setupNewLevel( UINT nDepth )
{
	// Clear old Map
	SAFE_FREE(m_pCaveGener);
	SAFE_FREE(m_caveVisited);

	// Go One Deeper
	m_playerWalk = NULL;
	m_caveDepth = nDepth;

	char buffer[10];
	sprintf_s(buffer, 10, "%02i", nDepth);
	m_pDepthMsg = TTF_RenderText_Solid(m_pDepthFont, buffer, WHITE_TEXT_COLOUR);

	// Generate Cave Size
	m_caveWidth = rand() % (MAP_MAX_SIDE - MAP_MIN_SIDE) + MAP_MIN_SIDE;
	m_caveHeight = MAX_MAP_POINTS / m_caveWidth;

	// Reset Visit
	m_caveVisited = new BYTE[m_caveWidth*m_caveHeight];
	memset(m_caveVisited, 0, sizeof(BYTE)*m_caveWidth*m_caveHeight);
	memset(m_caveItems, 0, sizeof(m_caveItems));

	// Generate Cave
	m_pCaveGener = new CaveGener(m_caveWidth, m_caveHeight);
	m_pCaveGener->GenerateCave(47, 9);
	m_pCaveGener->CountRooms();
	m_pCaveGener->ConnectRooms();
	m_pCaveGener->ProcessGeneration(9);
	m_pCaveGener->CountRooms();

	// Create Entrance & Exit
	FindEmptyPoint(m_caveItems[0].x, m_caveItems[0].y);
	FindEmptyPoint(m_caveItems[1].x, m_caveItems[1].y);
	m_caveItems[0].m_type = OBJ_ENTRANCE;
	m_caveItems[1].m_type = OBJ_EXIT;

	m_playerX = (m_caveItems[0].x * 16.0f) + 8.0f;
	m_playerY = (m_caveItems[0].y * 16.0f) + 8.0f;

	// Provide Several items
	UINT nItems = MAX_MAP_ITEMS;
	if(m_caveDepth < MAP_INSANSE_DEPTH)
	{
		nItems = ((MAX_MAP_ITEMS * m_caveDepth) / MAP_INSANSE_DEPTH);
	}
	
	for(UINT iItem = 2; iItem < nItems; ++iItem)
	{
		UINT chance = rand() % 100;

		FindEmptyPoint(m_caveItems[iItem].x, m_caveItems[iItem].y);
		
		if(chance < 50)
		{
			m_caveItems[iItem].m_type = OBJ_ITEM;
			m_caveItems[iItem].m_itemType = ITEM_GAS;
		}
		else
		{
			m_caveItems[iItem].m_type = OBJ_ITEM;
			m_caveItems[iItem].m_itemType = ITEM_STEAM;
		}
	}

	FREE_SURF(m_pCaveSurface);
	m_pCaveSurface = SDL_CreateRGBSurface( SDL_SWSURFACE, m_caveWidth*16, m_caveHeight*16, SCREEN_BPP, 0xFF000000, 0x00FF0000, 0x0000FF00, 0x000000FF);

	// Fill Solid Squares
	for(UINT x = 0; x < m_caveWidth; ++x)
	for(UINT y = 0; y < m_caveHeight; ++y)
	{
		if(m_pCaveGener->GetPoint(x,y) >= CAVE_DIRT)
		{
			SDL_Rect tileArea = { x*16,y*16,16,16};
			SDL_FillRect(m_pCaveSurface, &tileArea, 0x6F4E2DFF);
		}
	}
}

//////////////////////////////////////////////////////////////////////////
///	[] gameState::TraceWall
void gameState::TraceWall( UINT x, UINT y )
{
	struct pathPoint 
	{
		UINT x,y;
		pathPoint* pNext;
	};

	pathPoint* pStart = new pathPoint;
	pathPoint* pEnd = pStart;

	pStart->x = x;
	pStart->y = y;
	pStart->pNext = NULL;

	// Build Path
	while(m_caveVisited[x + (y*m_caveWidth)] == FALSE)
	{
		BYTE points[4] = 
		{
			(m_pCaveGener->GetPoint(x  ,y  ) >= CAVE_DIRT),
			(m_pCaveGener->GetPoint(x+1,y  ) >= CAVE_DIRT),
			(m_pCaveGener->GetPoint(x  ,y+1) >= CAVE_DIRT),
			(m_pCaveGener->GetPoint(x+1,y+1) >= CAVE_DIRT)
		};

		BYTE tileID = 
			(points[0] << 0) +
			(points[1] << 1) +
			(points[2] << 2) +
			(points[3] << 3);

		m_caveVisited[x + (y*m_caveWidth)] = TRUE;

		switch(tileID)
		{
		case  0:	return;	break;
		case  1:	--y;	break;
		case  2:	++x;	break;
		case  3:	++x;	break;
		case  4:	--x;	break;
		case  5:	--y;	break;
		case  6:	--x;	break;
		case  7:	++x;	break;
		case  8:	++y;	break;
		case  9:	--y;	break;
		case 10:	++y;	break;
		case 11:	++y;	break;
		case 12:	--x;	break;
		case 13:	--y;	break;
		case 14:	--x;	break;
		case 15:	return;	break;
		};

		pEnd->pNext = new pathPoint;
		pEnd = pEnd->pNext;
		pEnd->x = x;
		pEnd->y = y;
		pEnd->pNext = NULL;
	}

	// Trace Path
	pathPoint* pTemp = pStart;
	UINT srcX, srcY, destX, destY;

	srcX = (((pEnd->pNext->x - pEnd->x) + (pEnd->pNext->pNext->x - pEnd->x)) * 16 / 3) + pEnd->x;
	srcY = (((pEnd->pNext->y - pEnd->y) + (pEnd->pNext->pNext->y - pEnd->y)) * 16 / 3) + pEnd->y;

	while(pTemp != pEnd)
	{
		destX = (((pTemp->pNext->x - pTemp->x) + (pTemp->pNext->pNext->x - pTemp->x)) * 16 / 3) + pTemp->x;
		destY = (((pTemp->pNext->y - pTemp->y) + (pTemp->pNext->pNext->y - pTemp->y)) * 16 / 3) + pTemp->y;

		// Advance to the Next
		srcX = destX;
		srcY = destY;
		pTemp = pTemp->pNext;
	}

	// Clean up Path
	while(pStart != NULL)
	{
		pathPoint* pTemp = pStart;
		pStart = pStart->pNext;
		delete pTemp;
	}
}

//////////////////////////////////////////////////////////////////////////
///	[] gameState::isEmpty
bool gameState::isEmpty( UINT newPointX, UINT newPointY )
{
	if(m_pCaveGener->GetPoint(newPointX,newPointY) >= CAVE_DIRT)
	{
		return FALSE;
	}

	// Check Map items
	for(UINT iItem = 0; iItem < MAX_MAP_ITEMS; ++iItem)
	{
		if(m_caveItems[iItem].m_type != OBJ_NOT_IN_USE)
		{
			if( (m_caveItems[iItem].x == newPointX) &&
				(m_caveItems[iItem].y == newPointY))
			{
				return FALSE;
			}
		}
	}

	return TRUE;
}

//////////////////////////////////////////////////////////////////////////
///	[] gameState::FindEmptyPoint
void gameState::FindEmptyPoint( UINT& newPointX, UINT& newPointY )
{
	newPointX = rand() % m_caveWidth;
	newPointY = rand() % m_caveHeight;

	while(isEmpty(newPointX, newPointY) ==  FALSE)
	{
		newPointX += 1;

		if(newPointX >= m_caveWidth)
		{
			newPointX = 0;
			newPointY += 1;

			if(newPointY >= m_caveHeight)
			{
				newPointY = 0;
			}
		}
	}
}

//////////////////////////////////////////////////////////////////////////
///	[] gameState::UpdateShadows
void gameState::UpdateShadows()
{
	memset(m_pShadows, 0, sizeof(m_pShadows));
	/*
	int playX = (int)(m_playerX / 16.0f);
	int playY = (int)(m_playerY / 16.0f);
	int offsetX = playX - (SHADOW_SIZE_X / 2);
	int offsetY = playY - (SHADOW_SIZE_Y / 2);

	if(playX < (SHADOW_SIZE_X / 2)) { offsetX = 0; }
	if(playY < (SHADOW_SIZE_Y / 2)) { offsetY = 0; }
	if(playX > (m_caveWidth  - SHADOW_SIZE_X)) { offsetX = (m_caveWidth  - SHADOW_SIZE_X) + 1; }
	if(playY > (m_caveHeight - SHADOW_SIZE_Y)) { offsetY = (m_caveWidth  - SHADOW_SIZE_Y) + 1; }

	for(int x = 0; x < SHADOW_SIZE_X; ++x)
	for(int y = 0; y < SHADOW_SIZE_Y; ++y)
	{
		float pos[4] =
		{
			playX + 0.5f, 
			playY + 0.5f,
			offsetX + x + 0.5f,
			offsetY + y + 0.5f
		};

		float dir[2] = 
		{
			pos[2] - pos[0],
			pos[3] - pos[1],
		};
		
		BYTE bBlocking = 0;
		float length = sqrt(dir[0]*dir[0] + dir[1]*dir[1]);
		dir[0] = (dir[0] / length);
		dir[1] = (dir[1] / length);

		while((fabsf(pos[0] - pos[2]) > 0.8f) || (fabsf(pos[1] - pos[3]) > 0.8f))
		{
			float mod = 0.1f;

			// Check for Crossing
			while(((int)(pos[0]) == (int)(pos[0] + dir[0]*mod)) &&
				  ((int)(pos[1]) == (int)(pos[1] + dir[1]*mod)))
			{
				mod += 0.1f;
			}

			pos[0] += dir[0]*mod;
			pos[1] += dir[1]*mod;

			// Check if Free
			if(m_pCaveGener->GetPoint((int)(pos[0]),(int)(pos[1])) >= CAVE_DIRT)
			{
				++bBlocking;
			}

			if(bBlocking > 2)
			{
				break;
			}
		}

		m_pShadows[x + (y * SHADOW_SIZE_X)] = bBlocking;
	}
	/**/
}

//////////////////////////////////////////////////////////////////////////
///	[] gameState::RenderShadows
void gameState::RenderShadows( )
{
	for(UINT x = 0; x < SHADOW_SIZE_X; ++x)
	{
		for(UINT y = 0; y < SHADOW_SIZE_Y; ++y)
		{
			SDL_Rect shadowArea = 
			{
				(int)(x*16.0f),
				(int)(y*16.0f),
				16, 16
			};		

			SDL_Rect shadowLight = {16, 0,16,16 };
			SDL_Rect shadowMed   = { 0,16,16,16 };
			SDL_Rect shadowDark  = {16,16,16,16 };

			switch (m_pShadows[x + (y * SHADOW_SIZE_X)])
			{
			case 0:
				SDL_FillRect(m_pShadowMap, &shadowArea, 0x00000000);
				break;
			case 1:
				SDL_FillRect(m_pShadowMap, &shadowArea, 0x00000044);
				break;
			case 2:
				SDL_FillRect(m_pShadowMap, &shadowArea, 0x00000088);
				break;
			default:
				SDL_FillRect(m_pShadowMap, &shadowArea, 0x000000FF);
				break;
			};
			/**/
		}
	}

	SDL_UpdateRect(m_pShadowMap, 0, 0, m_pShadowMap->w, m_pShadowMap->h);
}

//////////////////////////////////////////////////////////////////////////
///	[] gameState::PrintMsg
void gameState::PrintMsg( const char* pString )
{
	m_ConsoleTimer = 4.0f;
	m_pConsoleMsg = TTF_RenderText_Solid(m_pMsgFont, pString, WHITE_TEXT_COLOUR);
}

//////////////////////////////////////////////////////////////////////////
///	[] gameState::UpdateKillCount
void gameState::UpdateKillCount()
{
	char buffer[50];
	sprintf_s(buffer, 50, "Kills: %3i", m_Kills);
	m_pScoreMsg = TTF_RenderText_Solid(m_pMsgFont, buffer, WHITE_TEXT_COLOUR);
}