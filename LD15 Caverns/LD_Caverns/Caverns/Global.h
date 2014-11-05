/********************************************************************
	created:	29:8:2009   11:04
	file path:	c:\Dev\LD_Caverns\Caverns\Global.h
	author:		Claire Blackshaw
	
	purpose:	Global header for types and junk
*********************************************************************/

#ifndef HEADER_INCLUDE_GLOBAL
#define HEADER_INCLUDE_GLOBAL

#include <SDL.h>
#include <SDL_image.h>
#include <SDL_ttf.h>
#include <SDL_mixer.h>
#include <string>

#undef  FALSE
#undef  TRUE
#define UINT unsigned int
#define FALSE	0
#define TRUE	1
#define MAX(a,b) (((a) > (b)) ? (a) : (b))
#define MIN(a,b) (((a) < (b)) ? (a) : (b))

#undef BYTE
#define BYTE unsigned char

#define CAVE_AIR	0x00
#define CAVE_DIRT	0x7D
#define CAVE_ROCK	0xFF

#define SCREEN_WIDTH	800
#define SCREEN_HEIGHT	600
#define SCREEN_BPP		32
#define SCREEN_TITLE	"LD:15 Steampunk Cavern Crawl into the Darkness - by Claire (Kimau)"

#define FIXED_FPS		30

#define FREE_SURF(surf)		if(surf != NULL) { SDL_FreeSurface(surf); surf = NULL; }
#define FREE_FONT(font)		if(font != NULL) { TTF_CloseFont(font);	font = NULL; }
#define SAFE_FREE(ptrr)		if(ptrr != NULL) { delete ptrr; ptrr = NULL; }

enum dirValue
{
	DIR_NORTH,
	DIR_EAST,
	DIR_SOUTH,
	DIR_WEST,
	NOOF_DIR
};

enum statePhase
{
	SP_DORMANT,
	SP_LOADING,
	SP_LOADED,
	SP_ACTIVE,
	SP_CLOSED,
	NOOF_STATE_PHASES
};

enum objectType
{
	OBJ_NOT_IN_USE,
	OBJ_ENTRANCE,
	OBJ_EXIT,
	OBJ_ITEM,
	OBJ_MONSTER,
	NOOF_OBJECT_TYPES
};

enum inventoryItems
{
	ITEM_NOT_IN_USE,
	ITEM_GAS,
	ITEM_STEAM,
	NOOF_INVENTORY_ITEMS
};

enum monsterType
{
	MONST_GRUNTER,
	MONST_TREX,
	NOOF_MONSTER_TYPES
};

struct MapItem 
{
	UINT		x,y;
	UINT		m_level;
	objectType	m_type;
	union
	{
		inventoryItems	m_itemType;
		monsterType		m_monType;
	};
};

#endif // HEADER_INCLUDE_GLOBAL