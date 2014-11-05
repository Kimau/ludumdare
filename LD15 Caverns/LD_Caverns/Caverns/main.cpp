/********************************************************************
	created:	29:8:2009   7:06
	file path:	c:\Dev\LD_Caverns\Caverns\main.cpp
	author:		Claire Blackshaw
	
	purpose:	Program Entry Point
*********************************************************************/

//////////////////////////////////////////////////////////////////////////
///	Game Includes
#include "GameClient.h"
#include "testState.h"
#include "gameState.h"

//////////////////////////////////////////////////////////////////////////
///	Programe Entry Point
int main(int argc, char *argv[])
{
	bClient gameClient;
	gameClient.StartClient();

	gameState gState;
	gameClient.PushState(&gState);

	while(gameClient.isRunning() == TRUE)
	{
		gameClient.UpdateEvents();	if(gameClient.isRunning() == FALSE) break;
		gameClient.UpdateLogic();	if(gameClient.isRunning() == FALSE) break;
		gameClient.RenderFrame();	if(gameClient.isRunning() == FALSE) break;
		gameClient.RestFrame();
	}

	gameClient.KillClient();

	return 0;
}
