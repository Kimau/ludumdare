/********************************************************************
	created:	29:8:2009   11:02
	file path:	c:\Dev\LD_Caverns\Caverns\CaveGener.h
	author:		Claire Blackshaw
	
	purpose:	Caver Generator
	algorithim sourced from 
	http://roguebasin.roguelikedevelopment.org/index.php?title=Cellular_Automata_Method_for_Generating_Random_Cave-Like_Levels
*********************************************************************/

#ifndef HEADER_INCLUDE_CAVEGENER
#define HEADER_INCLUDE_CAVEGENER

#include "Global.h"

///-------------------------------------
/// CaveGener
///
/// Brief: Cave Generator
///
///-------------------------------------
class CaveGener
{
public:
	//----------------------------------------------------- Public Functions
					CaveGener(int width, int height);
	virtual 		~CaveGener();

	virtual void	GenerateCave(int wallChance, UINT generations);
	virtual void	ProcessGeneration( UINT generations );
	virtual int		CountRooms();
	virtual void	ConnectRooms();

	BYTE			GetPoint(int x, int y) const;
	BYTE			GetFlipPoint(int x, int y) const;
	
	inline UINT		GetRoomCount() const { return m_nRooms; }

protected:
	//----------------------------------------------------- Protected Functions
private:
	//----------------------------------------------------- Private Functions
					CaveGener();
	void			RoomBackTrack( int x, int y, UINT thisRoom );

	//----------------------------------------------------- Private Variables
	BYTE*			m_pCaveData;
	BYTE*			m_pFlipData;	// Only valid during generation
	int				m_caveWidth;
	int				m_caveHeight;
	int				m_topEntrance;
	int				m_bottomExit;
	int				m_nRooms;
};

#endif // HEADER_INCLUDE_CAVEGENER