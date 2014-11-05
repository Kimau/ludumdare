#include "myMath.h"
#include "../include/pow2assert.h"

void mathHelper::wrap(float& in, float min, float max)
{
	while(in < min) { in += max - min; }
	while(in > max) { in += min - max; }
}

glm::vec3 mathHelper::polarVector(float theta, float phi, float rad)
{
	POW2_ASSERT(rad > 0);
	POW2_ASSERT((theta >= 0) && (theta <= (M_PI*2)));
	POW2_ASSERT((phi >= 0) && (phi <= (M_PI*2)));

	return glm::vec3(
		(rad * glm::sin(theta) * glm::cos(phi)),
		(rad * glm::cos(theta)),
		(rad * glm::sin(theta) * glm::sin(phi)) );
}

void mathHelper::buildLook(float theta, float phi, float rad, glm::vec3& lookOut, glm::vec3& upOut)
{
	lookOut = polarVector(theta, phi, rad);

	if (theta < 0.1)
	{
		float alteredPhi = phi + M_PI;
		if (alteredPhi > (M_PI*2))
		{
			alteredPhi -= (M_PI*2);
		}

		upOut = polarVector(theta, alteredPhi, rad);
	}
	else
	{
		float alteredTheate = theta - 0.1f;
		upOut = polarVector(alteredTheate, phi, rad);
	}

	upOut -= lookOut;
	upOut = glm::normalize(upOut);
}

void mathHelper::deconVector(glm::vec3 inVec, float& thetaOut, float& phiOut, float& radOut)
{
	thetaOut = 0;
	phiOut = 0;
	radOut = glm::length(inVec);

	if(radOut > 0)
	{
		phiOut = atan2f(inVec.z, inVec.x);
		thetaOut = glm::acos(inVec.y / radOut);
	}
}