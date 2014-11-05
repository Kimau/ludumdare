#ifndef SPRITE_CLASS_FOR_PTK_13
#define SPRITE_CLASS_FOR_PTK_13

#include <ptk.h>
#include <math.h>
#define S_MAXANI	5

//------------------------------------
//	Sprite Class
//------------------------------------
class pSprite
{
public:
	//-------------------------
	//	Creation
	//-------------------------	
	pSprite();
	pSprite(int _Width, int _Height, char* _file);
	~pSprite();

	int createSprite(int _Width, int _Height, char* _file);
	//-------------------------
	//	Mutation
	//-------------------------
	void nextFrame(float _advance);
	void setCycle(int _cycle, float _frame = -1);

	//-------------------------
	//	Accessors
	//-------------------------
	void draw(float x, float y) const;
	bool empty() const;

private:
	KGraphic* m_Sprite;		// KGraphic : Image Data
	int m_Width;			// Width of Sprite
	int m_Height;			// Height of Sprite
	int m_Length;			// Length of a Cycle
	int m_Cycle;			// Animation Cycle currently in process
	float m_Frame;			// Current Frame
};

#endif