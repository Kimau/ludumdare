/********************************************************************
	created:	29:8:2009   12:00
	file path:	c:\Dev\LD_Caverns\Caverns\baseState.h
	author:		Claire Blackshaw
	
	purpose:	Base State
*********************************************************************/

#ifndef HEADER_INCLUDE_BASESTATE
#define HEADER_INCLUDE_BASESTATE

#include "Global.h"

///-------------------------------------
/// baseState
///
/// Brief: Base State
///
///-------------------------------------
class baseState
{
public:
	//----------------------------------------------------- Public Functions
					baseState() {m_ePhase = SP_DORMANT; }
	virtual 		~baseState() {}

	virtual	bool	loadResources() = 0;	// Load Resources First
	virtual void	startState() = 0;		// Then Start the state
	virtual void	closeState() = 0;		// The Close the state to clean up and free resources

	virtual void	processEvent(SDL_Event& rEvent) = 0;
	virtual void	updateLogic() = 0;
	virtual void	renderFrame() = 0;

	statePhase		getPhase() const { return m_ePhase; }
protected:
	//----------------------------------------------------- Protected Variables
	statePhase		m_ePhase;
};

#endif // HEADER_INCLUDE_BASESTATE