#pragma once

#include "../glm/glm.hpp"

static const glm::vec3 s_ZeroVec(0.0f,0.0f,0.0f);
static const glm::vec3 s_OneVec(1.0f,1.0f,1.0f);
static const glm::vec3 s_XVec(1.0f,0.0f,0.0f);
static const glm::vec3 s_YVec(0.0f,1.0f,0.0f);
static const glm::vec3 s_ZVec(0.0f,0.0f,1.0f);

#define M_E        2.7182818f
#define M_LOG2E    1.4426950f
#define M_LOG10E   0.4342944f
#define M_LN2      0.6931471f
#define M_LN10     2.3025850f
#define M_2PI      6.2831852f
#define M_PI       3.1415926f
#define M_PI_2     1.5707963f
#define M_PI_4     0.7853981f
#define M_1_PI     0.3183098f
#define M_2_PI     0.6366197f
#define M_2_SQRTPI 1.1283791f
#define M_SQRT2    1.4142135f
#define M_SQRT1_2  0.7071067f

namespace mathHelper
{
	void wrap(float& in, float min, float max);
	glm::vec3 polarVector(float theta, float phi, float rad);
	void buildLook(float theta, float phi, float rad, glm::vec3& lookOut, glm::vec3& upOut);
	void deconVector(glm::vec3 inVec, float& thetaOut, float& phiOut, float& radOut);

}