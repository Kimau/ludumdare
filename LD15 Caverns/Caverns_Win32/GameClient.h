/********************************************************************
	created:	29:8:2009   7:08
	file path:	c:\Dev\LD_Caverns\Caverns\GameClient.h
	author:		Claire Blackshaw
	
	purpose:	Basic SDL Game Client thingy
*********************************************************************/

#ifndef HEADER_INCLUDE_GAMECLIENT
#define HEADER_INCLUDE_GAMECLIENT

// Includes
#include "Global.h"

class baseState;
struct stateStack 
{
	baseState*	m_pData;
	stateStack*	m_pBelow;
};

///-------------------------------------
/// bClient
///
/// Brief: Basic Client
///
///-------------------------------------
class bClient
{
public:
	//----------------------------------------------------- Public Functions
						bClient();
	virtual 			~bClient();

	// Creation and Destruction
	bool				StartClient();
	void				KillClient();

	void				PopState();
	void				PushState(baseState* pState);

	// Update Loop
	virtual void		UpdateEvents();
	virtual void		UpdateLogic();
	virtual void		RenderFrame();
	virtual void		RestFrame();

	// Resource Loading
	static SDL_Surface*	LoadTexture(const char* filename, bool bAlpha = FALSE, const SDL_Colour* pKey = NULL);
	static TTF_Font*	LoadFont(const char* filename, UINT ptSize);

	// Utility Functions
	static bClient*		GetClient();
	static UINT			TestPixel( SDL_Surface* pSurface, int x, int y );
	static void			DrawPixel( SDL_Surface* pSurface, int x, int y, UINT colour );
	static void			BlitSurface( SDL_Surface* pSource, SDL_Surface* pDest, UINT x = 0, UINT y = 0, SDL_Rect* clip = NULL);
	static void			Log(const char* pDebugString);

	// Accessors
	inline bool			isRunning() const		{ return m_bRunning; }
	inline float		deltaSeconds() const	{ return float(m_TimerCurr - m_TimerPrev) / 1000.0f; }
	inline float		runningSeconds() const	{ return float(m_TimerCurr - m_TimerStart) / 1000.0f; }
	inline SDL_Surface*	getScreen() const		{ return m_pBackBuffer; }
protected:
	//----------------------------------------------------- Protected Functions
private:
	//----------------------------------------------------- Private Functions
	//----------------------------------------------------- Private Variables
	SDL_Surface*			m_pBackBuffer;
	SDL_Surface*			m_pHACKIT;

	stateStack*				m_pStateStack;

	UINT					m_TimerStart;
	UINT					m_TimerPrev;
	UINT					m_TimerCurr;

	bool					m_bRunning;
};

#endif // HEADER_INCLUDE_GAMECLIENT