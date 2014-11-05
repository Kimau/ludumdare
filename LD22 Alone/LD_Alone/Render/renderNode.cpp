#include "renderNode.h"


renderNode::renderNode()
{
	m_isVisible = false;
}

renderNode::~renderNode()
{

}

void renderNode::renderCallList()
{
	glCallList(m_glList);
}
