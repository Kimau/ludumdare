#include "PolyHelper.h"

using namespace glm;

vec3 PolyHelper::closestPointToLine( vec3 point, vec3 a, vec3 b )
{
	point -= a;
	b -= a;

	float optDist = dot(point,b) / dot(b,b);

	return (a + optDist * b);
}

vec3 PolyHelper::closestPointToPlane( vec3 point, vec3 pNorm, vec3 pPoint )
{
	float optDist = dot( pNorm, (point - pPoint)) / dot(pNorm, pNorm);
	return (point - optDist * pNorm);
}

vec4 PolyHelper::intersectSegPlane( vec3 rA, vec3 rB, vec3 pNorm, vec3 pPoint )
{
	vec3 rayDir = rB - rA;
	vec3 w = rA - pPoint;

	float D = dot(pNorm, rayDir);
	if (fabs(D) < FLT_EPSILON) 
	{
		return vec4(0,0,0,0);
	}

	float optDist = dot(pNorm, w) / D;

	return vec4(rA - optDist * rayDir, optDist);
}

vec4 PolyHelper::intersectRayPlane( vec3 rayDir, vec3 pNorm, vec3 pPoint )
{
	float D = dot(pNorm, rayDir);
	if (fabs(D) < FLT_EPSILON) 
	{
		return vec4(0,0,0,0);
	}

	float optDist = dot(pNorm, -pPoint) / D;

	return vec4(optDist * rayDir, optDist);
}
