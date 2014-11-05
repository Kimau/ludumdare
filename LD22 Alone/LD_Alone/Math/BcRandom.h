/**************************************************************************
*
* File:		BcRandom.h
* Author: 	Neil Richardson 
* Ver/Date:	
* Description:
*		Marsaglia's MWC.
*		TODO: Perhaps implement something a bit, well, better?
*
*
* 
**************************************************************************/

#ifndef __BCRANDOM_H__
#define __BCRANDOM_H__

//////////////////////////////////////////////////////////////////////////
// BcRandom
class BcRandom
{
public:
	BcRandom();
	BcRandom( unsigned int Seed );

	unsigned int rand();
	float randReal();
	unsigned int randRange( unsigned int  Min, unsigned int  Max );
	float noise( unsigned int  X, unsigned int  Width );
	float smoothedNoise( float X, unsigned int  Width = 512 );
	float interpolatedNoise( float X, unsigned int  Width = 512 );

private:
	unsigned int Z_;
	unsigned int W_;
};

#endif