/********************************************************************
	created:	29:8:2009   7:09
	file path:	c:\Dev\LD_Caverns\Caverns\GameClient.cpp
	author:		Claire Blackshaw
	
	purpose:	
*********************************************************************/

#ifdef WIN32
#define WIN32_LEAN_AND_MEAN
#include <Windows.h>
#endif 

//////////////////////////////////////////////////////////////////////////
///	Game Includes
#include "GameClient.h"
#include "baseState.h"

static bClient* s_pClient = NULL;
bClient* bClient::GetClient()	{ return s_pClient; }

//////////////////////////////////////////////////////////////////////////
///	[] bClient::bClient
bClient::bClient()
{
	s_pClient = this;
	m_bRunning = FALSE;
	m_pBackBuffer = NULL;

	m_pStateStack = NULL;

	m_TimerStart = NULL;
	m_TimerPrev = NULL;
	m_TimerCurr = NULL;
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] bClient::~bClient
bClient::~bClient()
{

}

//////////////////////////////////////////////////////////////////////////
///	[] bClient::StartClient
bool bClient::StartClient()
{
	//Initialize all SDL subsystems
	if( SDL_Init( SDL_INIT_EVERYTHING ) == -1 )
	{
		return FALSE;
	}

	//Initialize SDL_ttf
	if( TTF_Init() == -1 )
	{
		return false;    
	}

	//Set up the screen
	m_pBackBuffer = SDL_SetVideoMode( SCREEN_WIDTH, SCREEN_HEIGHT, SCREEN_BPP, SDL_SWSURFACE );

	//If there was an error in setting up the screen
	if( m_pBackBuffer == NULL )
	{
		return FALSE;
	}

	//Set the window caption
	SDL_WM_SetCaption( SCREEN_TITLE, NULL );

	// Setup Timers
	m_TimerStart = m_TimerCurr = m_TimerPrev = SDL_GetTicks();

	// HACK
	m_pHACKIT = LoadTexture("TitleScreen.png");

	srand(m_TimerStart);

	m_bRunning = TRUE;
	return TRUE;
}

//////////////////////////////////////////////////////////////////////////
///	[] bClient::KillClient
void bClient::KillClient()
{
	// Kill State
	while(m_pStateStack != NULL)
	{
		m_pStateStack->m_pData->closeState();
		PopState();

	}

	// Free Fonts
	// Free Surfaces

	// Shutdown Systems
	TTF_Quit();
	SDL_Quit();

	m_bRunning = FALSE;
}

//////////////////////////////////////////////////////////////////////////
///	[] bClient::PopState
void bClient::PopState()
{
	stateStack* pTemp = m_pStateStack;
	m_pStateStack = m_pStateStack->m_pBelow;
	delete pTemp;
}

//////////////////////////////////////////////////////////////////////////
///	[] bClient::PushState
void bClient::PushState( baseState* pState )
{
	if(pState == NULL) return;

	stateStack* pTemp = new stateStack;
	pTemp->m_pBelow = m_pStateStack;
	pTemp->m_pData = pState;

	m_pStateStack = pTemp;
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] bClient::UpdateEvents
void bClient::UpdateEvents()
{
	SDL_Event PollEvent;

	while(SDL_PollEvent(&PollEvent))
	{
		if(PollEvent.type == SDL_QUIT)
		{
			KillClient();
			return;
		}

		// Pass Event to All active states
		if((m_pStateStack != NULL) && (m_pStateStack->m_pData->getPhase() == SP_ACTIVE))
		{
			m_pStateStack->m_pData->processEvent(PollEvent);
		}
	}
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] bClient::UpdateLogic
void bClient::UpdateLogic()
{
	// Update game timer
	m_TimerPrev = m_TimerCurr;
	m_TimerCurr = SDL_GetTicks();

	// Update all active state
	if((m_pStateStack != NULL) && (m_pStateStack->m_pData->getPhase() == SP_ACTIVE))
	{
		m_pStateStack->m_pData->updateLogic();
	}
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] bClient::RenderFrame
void bClient::RenderFrame()
{
	// HACK
	BlitSurface(m_pHACKIT, NULL);

	// Render All States
	if((m_pStateStack != NULL) && (m_pStateStack->m_pData->getPhase() == SP_ACTIVE))
	{
		m_pStateStack->m_pData->renderFrame();
	}

	// Flip Back Buffer
	if( SDL_Flip( m_pBackBuffer ) == -1 )
	{
		KillClient();
	}
}

//////////////////////////////////////////////////////////////////////////
///	[] bClient::LoadTexture
SDL_Surface* bClient::LoadTexture( const char* filename, bool bAlpha /*= FALSE*/, const SDL_Colour* pKey /*= NULL*/ )
{
	//The image that's loaded
	SDL_Surface* loadedImage = NULL;

	//The optimized image that will be used
	SDL_Surface* optimizedImage = NULL;

	//Load the image using SDL_image
	loadedImage = IMG_Load( filename );

	//If the image loaded
	if( loadedImage != NULL )
	{
		// Set Key
		if(pKey != NULL)
		{
			UINT colourKey = (pKey->r << 12) | (pKey->g << 8) | (pKey->b);
			SDL_SetColorKey(loadedImage, SDL_SRCCOLORKEY, colourKey);
		}

		//Create an optimized image
		if(bAlpha == TRUE)
		{
			optimizedImage = SDL_DisplayFormatAlpha( loadedImage );
		}
		else
		{
			optimizedImage = SDL_DisplayFormat( loadedImage );
		}

		//Free the old image
		FREE_SURF( loadedImage );
	}
	else
	{
		return NULL;
	}

	// TODO :: Track Resources here somehow

	// Return optimized surface
	return optimizedImage;
}

//////////////////////////////////////////////////////////////////////////
///	[] bClient::LoadFont
TTF_Font* bClient::LoadFont( const char* filename, UINT ptSize )
{
	TTF_Font* pFont = TTF_OpenFont(filename, ptSize);

	if( pFont == NULL )
	{
		return NULL;
	}

	// TODO :: Track Resources here somehow

	// Return Font Resource
	return pFont;
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] bClient::RestFrame
void bClient::RestFrame()
{
	if(m_pStateStack != NULL)
	{
		switch(m_pStateStack->m_pData->getPhase())
		{
		case SP_DORMANT:	
			m_pStateStack->m_pData->loadResources(); 
			break;
		case SP_LOADING:
			// Waiting on Thread
			break;
		case SP_LOADED:		
			m_pStateStack->m_pData->startState();	
			break;
		case SP_ACTIVE:
			// Do Nothing
			break;
		case SP_CLOSED:		
			PopState();
			break;
		}
	}

	// Rest for a while
	UINT presentTick = SDL_GetTicks();
	if((presentTick - m_TimerCurr) < (1000 / FIXED_FPS))
	{
		SDL_Delay((1000 / FIXED_FPS) - (presentTick - m_TimerCurr));
	}
}

//////////////////////////////////////////////////////////////////////////
///	[] bClient::TestPixel
UINT bClient::TestPixel( SDL_Surface* pSurface, int x, int y )
{
	UINT* pixArray = (UINT*)pSurface->pixels;
	return pixArray[ x + ( y * pSurface->w ) ];
}

//////////////////////////////////////////////////////////////////////////
///	[] bClient::DrawPixel
void bClient::DrawPixel( SDL_Surface* pSurface, int x, int y, UINT colour )
{
	UINT* pixArray = (UINT*)pSurface->pixels;
	pixArray[ x + ( y * pSurface->w ) ] = colour;
}

//////////////////////////////////////////////////////////////////////////
///	[] bClient::BlitSurface
void bClient::BlitSurface( SDL_Surface* pSource, SDL_Surface* pDest, UINT x /*= 0*/, UINT y /*= 0*/, SDL_Rect* clip /*= NULL*/ )
{
	// Setup Offset
	SDL_Rect offset;
	offset.x = x;
	offset.y = y;

	if(pDest == NULL)
	{
		pDest = s_pClient->m_pBackBuffer;
	}

	// Blit Surface
	SDL_BlitSurface( pSource, clip, pDest, &offset );
}

//////////////////////////////////////////////////////////////////////////
///	[] bClient::Log
void bClient::Log( const char* pDebugString )
{
#ifdef WIN32
	OutputDebugStringA(pDebugString);
#endif
}