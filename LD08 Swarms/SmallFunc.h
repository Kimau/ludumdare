#include <math.h>

struct pMoveable
{
	float m_pos[2];
	float m_dir[2];
	float m_accel[2];
	int m_frame;
};

struct pStatic
{
	bool m_walk;
	int m_pickup;
	float m_pos[2];
	float m_size[2];
	KGraphic* m_pic;
};

struct pXY
{
	float x;
	float y;
};

struct uXY
{
	int x;
	int y;
};

struct pSound
{
	float x;
	float y;
	float m_frame;
	float m_pow;
	float m_age;
};

float Magnitude(float x, float y)
{
	if((x) || (y))
	{
		return sqrtf(powf(x,2) + powf(y,2));
	}
	else return 0.0f;
}

float SqrdDist(const pMoveable& _first,const pMoveable& _second)
{
	float _dx = _first.m_pos[0] - _second.m_pos[0];
	float _dy = _first.m_pos[1] - _second.m_pos[1];
	return (powf(_dx,2) + powf(_dy,2));
};

void Norm(float& x, float& y, float _length = 1.0f)
{
	float _mag = Magnitude(x,y);
	if(_mag > 0.0000001f)
	{
		x = (x*_length) / _mag;
		y = (y*_length) /_mag;
	}
}