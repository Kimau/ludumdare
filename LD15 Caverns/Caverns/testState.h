/********************************************************************
	created:	29:8:2009   12:07
	file path:	c:\Dev\LD_Caverns\Caverns\testState.h
	author:		Claire Blackshaw
	
	purpose:	Test Bed for Crap
*********************************************************************/

#ifndef HEADER_INCLUDE_TESTSTATE
#define HEADER_INCLUDE_TESTSTATE

#include "Global.h"
#include "baseState.h"
#include "CaveGener.h"

///-------------------------------------
/// testState
///
/// Brief: 
///
///-------------------------------------
class testState : public baseState
{
public:
	//----------------------------------------------------- Public Functions
					testState();
	virtual 		~testState();

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
	static const UINT PIX_PER_SQUARE = 10;

	void			GenCaveSurface();
	void			UpdateCaveSurf();
	void			PrintStats();
	//----------------------------------------------------- Private Variables
	CaveGener*		m_pCaveGener;
	SDL_Surface*	m_pCaveSurface;
	SDL_Surface*	m_pRoomSurface;
	SDL_Surface*	m_pDebugMessage;
	TTF_Font*		m_pDebugFont;

	UINT			m_WallChance;
	UINT			m_Generations;

	UINT			m_MazeWidth;
	UINT			m_MazeHeight;

	bool			m_bDisplayRooms;
};

#endif // HEADER_INCLUDE_TESTSTATE