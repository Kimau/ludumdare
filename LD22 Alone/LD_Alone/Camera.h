#pragma once

#include "Math/myMath.h"

class Camera
{
public:
	Camera(void);
	~Camera(void);

	void BuildViewMatrix();

	void Orbit(float deltaPitch, float deltaYaw, float deltaRad);

	float		m_aspectRatio;
	float		m_FOV;
	glm::vec3	m_camPos;
	glm::vec3	m_camTarget;
	glm::vec3	m_camUp;
	glm::mat4	m_lastPerMat;
	glm::mat4	m_lastViewMat;
};

