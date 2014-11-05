#ifndef renderNode_h__
#define renderNode_h__

#include "global.h"

class renderNode
{
public:
	renderNode();
	~renderNode();

	void renderCallList();
	
private:
	bool			m_isVisible;
	glm::mat4x4		m_local;
	unsigned int	m_glList;
};

#endif // renderNode_h__
