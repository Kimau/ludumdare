/********************************************************************
	created:	29:8:2009   12:07
	file path:	c:\Dev\LD_Caverns\Caverns\testState.cpp
	author:		Claire Blackshaw
	
	purpose:	
*********************************************************************/

//////////////////////////////////////////////////////////////////////////
///	Game Includes
#include "testState.h"
#include "GameClient.h"

//////////////////////////////////////////////////////////////////////////
///	[] testState::testState
testState::testState()
{
	m_pCaveSurface = NULL;
	m_pDebugMessage = NULL;
	m_pDebugFont = NULL;
	m_pCaveGener = NULL;
	m_bDisplayRooms = FALSE;

	m_MazeWidth = 200;
	m_MazeHeight = 150;

	m_WallChance = 47;
	m_Generations = 9;
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] testState::~testState
testState::~testState()
{
	if((m_ePhase > SP_DORMANT) && (m_ePhase < SP_CLOSED))
	{
		closeState();
	}
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] testState::loadResources
bool testState::loadResources()
{
	m_ePhase = SP_LOADING;

	m_pDebugFont = bClient::LoadFont("5by5.ttf", 20);

	m_WallChance = 47;
	m_Generations = 9;

	m_bDisplayRooms = FALSE;
	m_MazeWidth = SCREEN_WIDTH / PIX_PER_SQUARE;
	m_MazeHeight = SCREEN_HEIGHT / PIX_PER_SQUARE;

	m_pCaveGener = new CaveGener(m_MazeWidth, m_MazeHeight);
	m_pCaveSurface = SDL_CreateRGBSurface( SDL_SWSURFACE, m_MazeWidth*PIX_PER_SQUARE, m_MazeHeight*PIX_PER_SQUARE, SCREEN_BPP, 0xFF0000, 0x00FF00, 0x0000FF, NULL );
	m_pRoomSurface = SDL_CreateRGBSurface( SDL_SWSURFACE, m_MazeWidth*PIX_PER_SQUARE, m_MazeHeight*PIX_PER_SQUARE, SCREEN_BPP, 0xFF0000, 0x00FF00, 0x0000FF, NULL );

	GenCaveSurface();

	m_ePhase = SP_LOADED;
	return TRUE;
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] testState::startState
void testState::startState()
{
	m_ePhase = SP_ACTIVE;
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] testState::closeState
void testState::closeState()
{
	FREE_FONT(m_pDebugFont);
	FREE_SURF(m_pRoomSurface);
	FREE_SURF(m_pCaveSurface);
	FREE_SURF(m_pDebugMessage);

	SAFE_FREE(m_pCaveGener);

	m_ePhase = SP_CLOSED;
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] testState::processEvent
void testState::processEvent( SDL_Event& rEvent )
{
	if(rEvent.type == SDL_KEYUP)
	{
		switch(rEvent.key.keysym.sym)
		{
		case SDLK_SPACE:
			GenCaveSurface();
			break;
		case SDLK_UP:
			m_WallChance = MAX(10, MIN(90, m_WallChance + 1));
			break;
		case SDLK_DOWN:
			m_WallChance = MAX(10, MIN(90, m_WallChance - 1));
			break;
		case SDLK_LEFT:
			m_Generations = MAX(1, MIN(20, m_Generations - 1));
			break;
		case SDLK_RIGHT:
			m_Generations = MAX(1, MIN(20, m_Generations + 1));
			break;
		case SDLK_r:
			m_bDisplayRooms = !m_bDisplayRooms;
			break;
		}

		PrintStats();
	}
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] testState::updateLogic
void testState::updateLogic()
{

}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] testState::renderFrame
void testState::renderFrame()
{
	if(m_bDisplayRooms == TRUE)
	{
		bClient::BlitSurface(m_pRoomSurface, NULL);		
	}
	else
	{
		bClient::BlitSurface(m_pCaveSurface, NULL);
	}

	bClient::BlitSurface(m_pDebugMessage, NULL);
}

//////////////////////////////////////////////////////////////////////////
///	[] testState::GenCaveSurface
void testState::GenCaveSurface()
{
	m_pCaveGener->GenerateCave(m_WallChance, m_Generations);
	m_pCaveGener->CountRooms();
	m_pCaveGener->ConnectRooms();
	m_pCaveGener->ProcessGeneration(9);
	m_pCaveGener->CountRooms();

	UpdateCaveSurf();
}

//////////////////////////////////////////////////////////////////////////
///	[] testState::PrintStats
void testState::PrintStats()
{
#ifdef _DEBUG
	// Debug Display
	SDL_Colour fontColour = { 0xFF, 0xFF, 0xFF, 0xFF };
	char buffer[100];
	sprintf_s(buffer, 100, "Walls: %02i   Generations: %2i  Rooms: %3d", m_WallChance, m_Generations, m_pCaveGener->GetRoomCount());
	m_pDebugMessage = TTF_RenderText_Solid(m_pDebugFont, buffer, fontColour);
#endif // _DEBUG
}

//////////////////////////////////////////////////////////////////////////
///	[] testState::UpdateCaveSurf
void testState::UpdateCaveSurf()
{
	PrintStats();

	if( SDL_MUSTLOCK( m_pCaveSurface ) ) SDL_LockSurface( m_pCaveSurface );
	if( SDL_MUSTLOCK( m_pRoomSurface ) ) SDL_LockSurface( m_pRoomSurface );

	for(UINT x = 0; x < m_MazeWidth; ++x)
		for(UINT y = 0; y < m_MazeHeight; ++y)
		{
			int pointVal = m_pCaveGener->GetPoint(x,y);

			// 		LIGHT 0xA6 7C 52
			// 		DARK  0x3B 22 0B
			// 		RANGE 0x6B 5A 47

			UINT dirtColour = (
				((0x3B + (pointVal * 0x6B / 0xFF)) << 16) |
				((0x22 + (pointVal * 0x5A / 0xFF)) <<  8) |
				((0x0B + (pointVal * 0x47 / 0xFF))      ));

			UINT roomColour = UINT(m_pCaveGener->GetFlipPoint(x,y)) * 5000;

			for(UINT u = x*PIX_PER_SQUARE; u < ((x+1)*PIX_PER_SQUARE); ++u)
				for(UINT v = y*PIX_PER_SQUARE; v < ((y+1)*PIX_PER_SQUARE); ++v)
				{
					bClient::DrawPixel(m_pCaveSurface, u, v, dirtColour);
					bClient::DrawPixel(m_pRoomSurface, u, v, roomColour);
				}
		}

		//Unlock surface
		if( SDL_MUSTLOCK( m_pCaveSurface ) ) SDL_UnlockSurface( m_pCaveSurface );
		if( SDL_MUSTLOCK( m_pRoomSurface ) ) SDL_UnlockSurface( m_pRoomSurface );

		SDL_UpdateRect(m_pCaveSurface, 0, 0, m_pCaveSurface->w, m_pCaveSurface->h);
		SDL_UpdateRect(m_pRoomSurface, 0, 0, m_pRoomSurface->w, m_pRoomSurface->h);
}