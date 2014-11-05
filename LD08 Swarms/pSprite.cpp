#include "pSprite.h"

//%%%%%%%%%%%%%%%%%%%%%%%%%%%
//%%%%%%%%%%%%%%%%%%%%%%%%%%%
//		CREATION
//%%%%%%%%%%%%%%%%%%%%%%%%%%%
//%%%%%%%%%%%%%%%%%%%%%%%%%%%
//================================================================================
//	Blank Creation
//================================================================================
pSprite::pSprite()
{
	m_Width  = 0;
	m_Height = 0;
	m_Cycle  = 0;
	m_Frame  = 0.0f;
	m_Length = 0;
	m_Sprite = 0;
}
//================================================================================
//	Create Sprite
//================================================================================
pSprite::pSprite(int _Width, int _Height, char* _file)
{
	m_Width  = _Width;
	m_Height = _Height;
	m_Cycle = 0;
	m_Frame = 0.0f;
	m_Sprite = KPTK::createKGraphic();
	m_Sprite->loadPicture(_file, true, true);
	m_Length = (int)(m_Sprite->getWidth() / _Width);
}
//================================================================================
//	Clean up After yourself Piggie
//================================================================================
pSprite::~pSprite()
{
	if(m_Sprite)
		delete m_Sprite;
}
//================================================================================
//	Create Sprite (Deletes Old one if present)
//================================================================================
int pSprite::createSprite(int _Width, int _Height, char* _file)
{
	//------------------
	//	Delete Old
	//------------------
	if(m_Sprite)
		delete m_Sprite;
	//---------------------------
	//	Create New
	//---------------------------
	m_Width  = _Width;
	m_Height = _Height;
	m_Length = 0;	// TEMP
	m_Cycle = 0;
	m_Frame = 0;
	m_Sprite = KPTK::createKGraphic();
	if(m_Sprite->loadPicture(_file, true, true))
	{
		m_Length = (int)(m_Sprite->getWidth() / _Width);
		return 1;
	}
	else return 0;
}
//%%%%%%%%%%%%%%%%%%%%%%%%%%%
//%%%%%%%%%%%%%%%%%%%%%%%%%%%
//		MUTATION
//%%%%%%%%%%%%%%%%%%%%%%%%%%%
//%%%%%%%%%%%%%%%%%%%%%%%%%%%
//================================================================================
//	Update the sprite animation
//	This is based on the elapsed time and FPS
//================================================================================
void pSprite::nextFrame(float _advance)
{
	m_Frame += _advance;
	while((m_Frame) >= (m_Length - 0.1f))
	{
		m_Frame -= m_Length;
	}
}
//================================================================================
//	Set Current Animation Cycle
//================================================================================
void pSprite::setCycle(int _cycle, float _frame)
{
	m_Cycle = _cycle;
	if(_frame > 0)
		m_Frame = _frame;
}
//%%%%%%%%%%%%%%%%%%%%%%%%%%%
//%%%%%%%%%%%%%%%%%%%%%%%%%%%
//		ACCESSORS
//%%%%%%%%%%%%%%%%%%%%%%%%%%%
//%%%%%%%%%%%%%%%%%%%%%%%%%%%
//================================================================================
//	Get Position as Vector
//================================================================================
void pSprite::draw(float x, float y) const
{
	m_Sprite->blitAlphaRect(floorf(m_Frame)  *m_Width, m_Cycle   *m_Height,
							floorf(m_Frame+1)*m_Width,(m_Cycle+1)*m_Height,
							x - (m_Width/2), y - (m_Height/2), 
							false, false);
}
//================================================================================
//	Is Sprite Empty
//================================================================================
bool pSprite::empty() const
{
	return (m_Sprite == 0);
}