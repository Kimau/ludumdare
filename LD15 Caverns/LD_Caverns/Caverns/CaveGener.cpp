/********************************************************************
	created:	29:8:2009   11:06
	file path:	c:\Dev\LD_Caverns\Caverns\CaveGener.cpp
	author:		Claire Blackshaw
	
	purpose:	
*********************************************************************/

//////////////////////////////////////////////////////////////////////////
///	Game Includes
#include "CaveGener.h"
#include "GameClient.h"

//////////////////////////////////////////////////////////////////////////
///	[] CaveGener::CaveGener
CaveGener::CaveGener( int width, int height )
{
	m_caveWidth = width;
	m_caveHeight = height;
	m_nRooms = NULL;
	
	m_topEntrance = rand() % width;
	m_bottomExit = rand() % width;

	m_pFlipData = new BYTE[m_caveWidth*m_caveHeight];
	m_pCaveData = new BYTE[m_caveWidth*m_caveHeight];
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] CaveGener::~CaveGener
CaveGener::~CaveGener()
{
	delete[] m_pFlipData;
	delete[] m_pCaveData;

	m_pFlipData = NULL;
	m_pCaveData	= NULL;
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] CaveGener::GenerateCave
void CaveGener::GenerateCave( int wallChance, UINT generations )
{
	bClient::Log("\n ------------------------- \n GENERATE CAVES \n ------------------------- \n" );

	m_nRooms = NULL;

	memset(m_pCaveData, 0, sizeof(BYTE)*m_caveWidth*m_caveHeight);

	// Init with random noise
	for(int y = 0; y < m_caveHeight; ++y)
	{
		for(int x = 0; x < m_caveWidth; ++x)
		{
			if( (y == 0) || 
				(x == 0) || 
				(y == (m_caveHeight - 1)) ||
				(x == (m_caveWidth - 1)))
			{
				// Borders are Rock
				m_pCaveData[x+(y*m_caveWidth)] = CAVE_DIRT;
			}
			else if((rand() % 100) < wallChance)
			{
				m_pCaveData[x+(y*m_caveWidth)] = CAVE_DIRT;
			}
			else
			{
				m_pCaveData[x+(y*m_caveWidth)] = CAVE_AIR;
			}
		}
	}

	ProcessGeneration(generations);

}

//////////////////////////////////////////////////////////////////////////
///	[] CaveGener::GetPoint
BYTE CaveGener::GetPoint( int x, int y ) const
{
	// Check Bounds
	if( (x < 0) || (x >= m_caveWidth) ||
		(y < 0) || (y >= m_caveHeight))
	{
		return CAVE_ROCK;
	}
	else
	{
		return m_pCaveData[x+(y*m_caveWidth)];
	}
}

//////////////////////////////////////////////////////////////////////////
///	[] CaveGener::GetFlipPoint
BYTE CaveGener::GetFlipPoint( int x, int y ) const
{
	// Check Bounds
	if( (x < 0) || (x >= m_caveWidth) ||
		(y < 0) || (y >= m_caveHeight))
	{
		return CAVE_ROCK;
	}
	else
	{
		return m_pFlipData[x+(y*m_caveWidth)];
	}
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] CaveGener::CountRooms
int CaveGener::CountRooms()
{
	bClient::Log("\n ------------------------- \n COUNT ROOMS \n ------------------------- \n" );
	m_nRooms = NULL;
	char buffer[512];

	memset(m_pFlipData, 0xFF, sizeof(BYTE)*m_caveWidth*m_caveHeight);

	for(int y = 0; y < m_caveHeight; ++y)
	{
		for(int x = 0; x < m_caveWidth; ++x)
		{
			if( (GetPoint(x,y) == CAVE_AIR) && 
				(m_pFlipData[x+(y*m_caveWidth)] >= 0xF0))
			{
				// Check Above
				if(GetPoint(x,y-1) == CAVE_AIR)
				{
					UINT thisRoom = m_pFlipData[x+((y-1)*m_caveWidth)];
					RoomBackTrack(x, y, thisRoom);
				}
				// Check to the left
				else if(GetPoint(x-1,y) == CAVE_AIR)
				{
					m_pFlipData[x+(y*m_caveWidth)] = m_pFlipData[(x-1)+(y*m_caveWidth)];
				}
				else
				{
					// ++m_nRooms;
					m_pFlipData[x+(y*m_caveWidth)] = m_nRooms;
					RoomBackTrack(x, y, m_nRooms);
					++m_nRooms;

					sprintf_s(buffer, 512, "Create Room %i \t Count is %i \n", m_nRooms, m_nRooms);
					bClient::Log(buffer);
				}
			}
		}
	}

	return m_nRooms;
}

//////////////////////////////////////////////////////////////////////////
///	[] CaveGener::RoomBackTrack
void CaveGener::RoomBackTrack( int x, int y, UINT thisRoom )
{
	m_pFlipData[x+(y*m_caveWidth)] = thisRoom;

	// Track Back & Up
	int backX = x-1;
	while(GetPoint(backX,y) == CAVE_AIR)
	{
		m_pFlipData[backX+(y*m_caveWidth)] = thisRoom;

		if( (GetPoint(backX,y-1) == CAVE_AIR) && 
			(m_pFlipData[backX+((y-1)*m_caveWidth)] != thisRoom))
		{
			RoomBackTrack(backX, (y-1), thisRoom);
		}

		if( (GetPoint(backX,y+1) == CAVE_AIR) && 
			(m_pFlipData[backX+((y+1)*m_caveWidth)] != thisRoom))
		{
			RoomBackTrack(backX, (y+1), thisRoom);
		}

		--backX;
	}

	// Track Forward
	int forwardX = x+1;
	while(GetPoint(forwardX,y) == CAVE_AIR)
	{
		m_pFlipData[forwardX+(y*m_caveWidth)] = thisRoom;

		if( (GetPoint(forwardX,y-1) == CAVE_AIR) && 
			(m_pFlipData[forwardX+((y-1)*m_caveWidth)] != thisRoom))
		{
			RoomBackTrack(forwardX, (y-1), thisRoom);
		}

		if( (GetPoint(forwardX,y+1) == CAVE_AIR) && 
			(m_pFlipData[forwardX+((y+1)*m_caveWidth)] != thisRoom))
		{
			RoomBackTrack(forwardX, (y+1), thisRoom);
		}

		++forwardX;
	}
}

//////////////////////////////////////////////////////////////////////////
///	[virtual ] CaveGener::ConnectRooms
void CaveGener::ConnectRooms()
{
	struct drillShaft 
	{
		BYTE	m_dir;	// NORTH, EAST, SOUTH, WEST
		int		x,y;
		int		count;
	};

	BYTE foundRoom = 0;
	int nStep = 0;
	bool bFound = FALSE;
	drillShaft aRooms[100];
	memset(aRooms, 0, sizeof(drillShaft)*100);

	// Check All Rooms
	for(int RoomID = 0; RoomID < 50; ++RoomID)
	{
		aRooms[RoomID].count = -1;
		nStep = 0;
		bFound = FALSE;
		foundRoom = 0;

		//_________________________________________________________ NORTH SWEEP
		for(int y = (m_caveHeight-1); y >= 0; --y)
		{
			for(int x = 0; x < m_caveWidth; ++x)
			{
				if(GetFlipPoint(x,y) == RoomID)
				{
					foundRoom = 1;
				}
			}

			if(foundRoom == 1)
			{
				foundRoom = 2;
			}
			else if(foundRoom == 2)
			{
				foundRoom = 0;

				for(int x = 0; x < m_caveWidth; ++x)
				{
					if(GetFlipPoint(x,y+1) == RoomID)
					{
						nStep = 0;

						while(((y-nStep) >= 0) && (bFound == FALSE))
						{
							// Broke Through
							if((GetPoint(x,(y-nStep)) < CAVE_DIRT) && (GetFlipPoint(x,(y-nStep)) != RoomID))
							{
								bFound = TRUE;
							}
							else
							{
								++nStep;
							}
						}

						// Is this shaft better
						if( (bFound == TRUE) && 
							(aRooms[RoomID].count < nStep))
						{
							aRooms[RoomID].x = x;
							aRooms[RoomID].y = y;
							aRooms[RoomID].m_dir = DIR_NORTH;
							aRooms[RoomID].count = nStep;
						}
					}
				}
			}
		}

		//_________________________________________________________ EAST  SWEEP
		for(int x = 0; x < m_caveWidth; ++x)
		{
			for(int y = 0; y < m_caveHeight; ++y)
			{
				if(GetFlipPoint(x,y) == RoomID)
				{
					foundRoom = 1;
				}
			}

			if(foundRoom == 1)
			{
				foundRoom = 2;
			}
			else if(foundRoom == 2)
			{
				foundRoom = 0;

				for(int y = 0; y < m_caveHeight; ++y)
				{
					if(GetFlipPoint(x-1,y) == RoomID)
					{
						nStep = 0;

						while(((x+nStep) < m_caveWidth) && (bFound == FALSE))
						{
							// Broke Through
							if((GetPoint((x+nStep),y) < CAVE_DIRT) && (GetFlipPoint((x+nStep),y) != RoomID))
							{
								bFound = TRUE;
							}
							else
							{
								++nStep;
							}
						}

						// Is this shaft better
						if( (bFound == TRUE) && 
							(aRooms[RoomID].count < nStep))
						{
							aRooms[RoomID].x = x;
							aRooms[RoomID].y = y;
							aRooms[RoomID].m_dir = DIR_EAST;
							aRooms[RoomID].count = nStep;
						}
					}
				}
			}
		}

		//_________________________________________________________ SOUTH SWEEP
		for(int y = 0; y < m_caveHeight; ++y)
		{
			for(int x = 0; x < m_caveWidth; ++x)
			{
				if(GetFlipPoint(x,y) == RoomID)
				{
					foundRoom = 1;
				}
			}

			if(foundRoom == 1)
			{
				foundRoom = 2;
			}
			else if(foundRoom == 2)
			{
				foundRoom = 0;

				for(int x = 0; x < m_caveWidth; ++x)
				{
					if(GetFlipPoint(x,y-1) == RoomID)
					{
						nStep = 0;

						while(((y+nStep) < m_caveHeight) && (bFound == FALSE))
						{
							// Broke Through
							if((GetPoint(x,(y+nStep)) < CAVE_DIRT) && (GetFlipPoint(x,(y+nStep)) != RoomID))
							{
								bFound = TRUE;
							}
							else
							{
								++nStep;
							}
						}

						// Is this shaft better
						if( (bFound == TRUE) && 
							(aRooms[RoomID].count < nStep))
						{
							aRooms[RoomID].x = x;
							aRooms[RoomID].y = y;
							aRooms[RoomID].m_dir = DIR_SOUTH;
							aRooms[RoomID].count = nStep;
						}
					}
				}
			}
		}

		//_________________________________________________________ WEST  SWEEP
		for(int x = (m_caveWidth-1); x >= 0; --x)
		{
			for(int y = 0; y < m_caveHeight; ++y)
			{
				if(GetFlipPoint(x,y) == RoomID)
				{
					foundRoom = 1;
				}
			}

			if(foundRoom == 1)
			{
				foundRoom = 2;
			}
			else if(foundRoom == 2)
			{
				foundRoom = 0;

				for(int y = 0; y < m_caveHeight; ++y)
				{
					if(GetFlipPoint(x+1,y) == RoomID)
					{
						nStep = 0;

						while(((x-nStep) >= 0) && (bFound == FALSE))
						{
							// Broke Through
							if((GetPoint((x-nStep),y) < CAVE_DIRT) && (GetFlipPoint((x-nStep),y) != RoomID))
							{
								bFound = TRUE;
							}
							else
							{
								++nStep;
							}
						}

						// Is this shaft better
						if( (bFound == TRUE) && 
							(aRooms[RoomID].count < nStep))
						{
							aRooms[RoomID].x = x;
							aRooms[RoomID].y = y;
							aRooms[RoomID].m_dir = DIR_EAST;
							aRooms[RoomID].count = nStep;
						}
					}
				}
			}
		}

		// Drop Shaft
		if(aRooms[RoomID].count >= 0)
		{
			int dx, dy;
			int stepX = aRooms[RoomID].x;
			int stepY = aRooms[RoomID].y;
			int nStep = 0;

			switch(aRooms[RoomID].m_dir)
			{
			case DIR_NORTH:		dx = 0;		dy =-1;		break;
			case DIR_EAST:		dx =+1;		dy = 0;		break;
			case DIR_SOUTH:		dx = 0;		dy =+1;		break;
			case DIR_WEST:		dx =-1;		dy = 0;		break;
			}

			while((GetPoint(stepX, stepY) != CAVE_ROCK) &&
				  (nStep < aRooms[RoomID].count))
			{
				if( (GetPoint(stepX, stepY) < CAVE_DIRT) && 
					(GetFlipPoint(stepX, stepY) != RoomID))
				{
					break;
				}

				m_pCaveData[stepX + (stepY*m_caveWidth)] = CAVE_AIR;
				m_pFlipData[stepX + (stepY*m_caveWidth)] = RoomID;

				if(dx == 0)
				{
					if(GetPoint((stepX-1), stepY) == CAVE_DIRT)
					{
						m_pCaveData[(stepX-1) + (stepY*m_caveWidth)] = CAVE_AIR;
						m_pFlipData[(stepX-1) + (stepY*m_caveWidth)] = RoomID;
					}

					if(GetPoint((stepX+1), stepY) == CAVE_DIRT)
					{
						m_pCaveData[(stepX+1) + (stepY*m_caveWidth)] = CAVE_AIR;
						m_pFlipData[(stepX+1) + (stepY*m_caveWidth)] = RoomID;
					}
				}
				else
				{
					if(GetPoint(stepX, (stepY-1)) == CAVE_DIRT)
					{
						m_pCaveData[(stepX-1) + (stepY*m_caveWidth)] = CAVE_AIR;
						m_pFlipData[(stepX-1) + (stepY*m_caveWidth)] = RoomID;
					}

					if(GetPoint(stepX, (stepY+1)) == CAVE_DIRT)
					{
						m_pCaveData[stepX + ((stepY+1)*m_caveWidth)] = CAVE_AIR;
						m_pFlipData[stepX + ((stepY+1)*m_caveWidth)] = RoomID;
					}
				}

				// Advance
				++nStep;
				stepX = aRooms[RoomID].x + (dx * nStep);
				stepY = aRooms[RoomID].y + (dy * nStep);
			}
		}
	}

}

//////////////////////////////////////////////////////////////////////////
///	[] CaveGener::ProcessGeneration
void CaveGener::ProcessGeneration( UINT generations )
{
	UINT firstLoop = MAX(2, (generations / 2));
	bool bFlipTarget = FALSE;
	BYTE* pSrc = NULL;
	BYTE* pDest = NULL;
	UINT closeCutoff = 5;
	UINT farCutoff = 2;

	// Check 5x5 square
	for(UINT iLoop = 0; iLoop < generations; ++iLoop)
	{
		// Setup Surfaces
		if(bFlipTarget == FALSE)
		{
			pSrc = m_pCaveData;
			pDest = m_pFlipData;
		}
		else
		{
			pSrc = m_pFlipData;
			pDest = m_pCaveData;
		}

		// Copy to Target
		memcpy(pDest, pSrc, sizeof(BYTE)*m_caveWidth*m_caveHeight);

		// For every non-border square
		for(int y = 1; y < (m_caveHeight - 1); ++y)
			for(int x = 1; x < (m_caveWidth - 1); ++x)
			{
				UINT wallCount = 0;

				// Count Close Walls 3x3	
				for(int stepX = x-1; stepX <= (x+1); ++stepX)
					for(int stepY = y-1; stepY <= (y+1); ++stepY)
					{
						wallCount += (pSrc[stepX + (stepY*m_caveWidth)] >= CAVE_DIRT);
					}

					// Surrounded by walls... CONFORM!!!
					if(wallCount >= closeCutoff)
					{
						pDest[x + (y*m_caveWidth)] = CAVE_DIRT;
					}
					else if(generations < firstLoop)
					{
						UINT farWallCount = 0;

						// Count Far Walls
						for(int stepX = x-2; stepX <= (x+2); ++stepX)
							for(int stepY = y-2; stepY <= (y+2); ++stepY)
							{
								// Don't want corners
								if((abs(stepX - x) == 2) && (abs(stepY - y) == 2))
								{
									continue;
								}

								// Check Bounds
								if( (stepX < 0) || (stepX >= m_caveWidth) ||
									(stepY < 0) || (stepY >= m_caveHeight))
								{
									continue;
								}

								wallCount += (pSrc[stepX + (stepY*m_caveWidth)] >= CAVE_DIRT);
							}

							// Not enough walls in 5x5 Area
							if(farWallCount <= farCutoff)
							{
								pDest[x + (y*m_caveWidth)] = CAVE_DIRT;
							}
							else
							{
								pDest[x + (y*m_caveWidth)] = CAVE_AIR;
							}
					}
					else
					{
						pDest[x + (y*m_caveWidth)] = CAVE_AIR;
					}

			}

			// Done with generation
			bFlipTarget = !bFlipTarget;
	}

	// Clean up : Safety check in case of odd generations
	if(bFlipTarget == TRUE)
	{
		BYTE* pTemp = m_pCaveData;
		m_pCaveData = m_pFlipData;
		m_pFlipData = pTemp;
	}
}