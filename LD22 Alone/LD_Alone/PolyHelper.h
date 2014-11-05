#pragma once

#include "glm/glm.hpp"

class PolyHelper
{
public:
	PolyHelper(void) {}
	~PolyHelper(void) {}


	static glm::vec3 closestPointToLine( glm::vec3 point, glm::vec3 a, glm::vec3 b );
	static glm::vec3 closestPointToPlane( glm::vec3 point, glm::vec3 pNorm, glm::vec3 pPoint);
	static glm::vec4 intersectSegPlane( glm::vec3 rA, glm::vec3 rB, glm::vec3 pNorm, glm::vec3 pPoint);
	static glm::vec4 intersectRayPlane( glm::vec3 rayDir, glm::vec3 pNorm, glm::vec3 pPoint);
};

