/**************************************************************************
*
* File:		BcRandom.cpp
* Author: 	Neil Richardson 
* Ver/Date:	
* Description:
*		Marsaglia's MWC.
*		TODO: Perhaps implement something a bit, well, better?
*
*
* 
**************************************************************************/

#include "BcRandom.h"

//////////////////////////////////////////////////////////////////////////
// Ctor
BcRandom::BcRandom()
{
	Z_ = 0x0000dead;
	W_ = 0x0000beef;
}

//////////////////////////////////////////////////////////////////////////
// Ctor
BcRandom::BcRandom( unsigned int Seed )
{
	Z_ = Seed & 65535;
	W_ = Seed >> 16;
}

//////////////////////////////////////////////////////////////////////////
// rand
unsigned int BcRandom::rand()
{
	Z_ = 36969 * ( Z_ & 65535 ) + ( Z_ >> 16 );
	W_ = 18000 * ( W_ & 65535 ) + ( W_ >> 16 );

	return ( Z_ << 16 ) + W_;
}

//////////////////////////////////////////////////////////////////////////
// randReal
float BcRandom::randReal()
{
	return float( rand() ) * 2.328306435996595e-10f;
}

//////////////////////////////////////////////////////////////////////////
// randRange
unsigned int BcRandom::randRange( unsigned int Min, unsigned int Max )
{
	return ( Min + ( rand() % ( Max - Min ) ) );
}

//////////////////////////////////////////////////////////////////////////
// noise
float BcRandom::noise( unsigned int X, unsigned int Width )
{
	X = X % Width;
	X = ( X << 13 ) ^ X;
	return ( 1.0f - ( ( X * ( X * X * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0f );
}

//////////////////////////////////////////////////////////////////////////
// smoothedNoise
float BcRandom::smoothedNoise( float X, unsigned int Width )
{
	return noise( (unsigned int)X, Width ) / 2.0f + noise( (unsigned int)X - 1, Width ) / 4 + noise( (unsigned int)X + 1, Width ) / 4;
}

//////////////////////////////////////////////////////////////////////////
// interpolatedNoise
float BcRandom::interpolatedNoise( float X, unsigned int Width )
{
	unsigned int iX = unsigned int( X );
	float FracX = X - iX;
	float V1 = smoothedNoise( (float)iX, Width );
	float V2 = smoothedNoise( (float)iX + 1.0f, Width );
	return V1 + ( V2 - V1 ) * FracX;
}