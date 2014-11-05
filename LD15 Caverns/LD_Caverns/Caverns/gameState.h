/********************************************************************
	created:	30:8:2009   12:28
	file path:	c:\Dev\LD_Caverns\Caverns\gameState.h
	author:		Claire Blackshaw
	
	purpose:	The Cavern Crawl Game State
*********************************************************************/

#ifndef HEADER_INCLUDE_GAMESTATE
#define HEADER_INCLUDE_GAMESTATE

// Includes
#include "baseState.h"

class CaveGener;

///-------------------------------------
/// gameState
///
/// Brief: Basic Game State
///
///-------------------------------------
class gameState : public baseState
{
public:
	//----------------------------------------------------- Public Functions
					gameState();
	virtual 		~gameState();

	virtual	bool	loadResources();	// Load Resources First
	virtual void	startState();		// Then Start the state
	virtual void	closeState();		// The Close the state to clean up and free resources

	virtual void	processEvent(SDL_Event& rEvent);
	virtual void	updateLogic();
	virtual void	renderFrame();

protected:
	//----------------------------------------------------- Protected Functions
private:
	//----------------------------------------------------- Private Functions
	static const UINT MAX_INVENTORY = 10;
	static const UINT MAX_MAP_ITEMS = 20;
	static const UINT MAX_MAP_POINTS = 4800;
	static const UINT MAP_MIN_SIDE = 40;
	static const UINT MAP_MAX_SIDE = 100;
	static const UINT MAP_INSANSE_DEPTH = 20;
	static const int SHADOW_SIZE_X = 40;
	static const int SHADOW_SIZE_Y = 40;

	void			setupNewPlayer(const char* pName, UINT nDifficulty);
	void			setupNewLevel(UINT nDepth);
	void			TraceWall( UINT x, UINT y );

	void			FindEmptyPoint( UINT& newPointX, UINT& newPointY );
	bool			isEmpty( UINT newPointX, UINT newPointY );

	void			CalculateScroll( SDL_Rect &tileArea, SDL_Rect &playSprite);
	void			RenderMapObjects( SDL_Rect &tileArea );
	void			UpdateShadows();
	void			RenderShadows();
	void			PrintMsg( const char* pString );
	void			UpdateKillCount();

	//----------------------------------------------------- Private Variables
	SDL_Surface*	m_pGUISurface;
	SDL_Surface*	m_pCaveSurface;
	SDL_Surface*	m_pShadowTiles;
	SDL_Surface*	m_pShadowMap;
	SDL_Surface*	m_pPlayerSprite;
	SDL_Surface*	m_pMonsterImage;
	SDL_Surface*	m_pRestartMsg;
	SDL_Surface*	m_pDeathMsg;
	SDL_Surface*	m_pDepthMsg;
	SDL_Surface*	m_pScoreMsg;
	SDL_Surface*	m_pConsoleMsg;
	SDL_Surface*	m_pInventory;

	TTF_Font*		m_pMsgFont;
	TTF_Font*		m_pDepthFont;

	// Player Information
	dirValue		m_playerFace;
	UINT			m_playerWalk;
	float			m_playerX;
	float			m_playerY;
	float			m_playerFrame;
	float			m_SteamPower;
	float			m_FuelReserve;
	UINT			m_MonsterChance;
	UINT			m_Kills;
	inventoryItems	m_pItems[MAX_INVENTORY];
	BYTE			m_pShadows[SHADOW_SIZE_X*SHADOW_SIZE_Y];

	// Map Information
	CaveGener*		m_pCaveGener;
	UINT			m_caveDepth;
	UINT			m_caveWidth;
	UINT			m_caveHeight;
	MapItem			m_caveItems[MAX_MAP_ITEMS];
	BYTE*			m_caveVisited;

	bool			m_bDead;
	bool			m_bShadows;
	float			m_ConsoleTimer;
};

#endif // HEADER_INCLUDE_GAMESTATE