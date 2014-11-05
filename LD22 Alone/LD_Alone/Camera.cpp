#include "global.h"
#include "Camera.h"
#include "include/GL/glew.h"
#include "include/GL/glfw.h"
#include "glm/gtc/matrix_transform.hpp"

using namespace glm;

Camera::Camera(void) : m_aspectRatio(8.0f / 6.0f), m_FOV(50.0f)
{
	m_camPos = s_OneVec * 50.0f;
	m_camTarget = s_ZeroVec;
	m_camUp = s_YVec;
}


Camera::~Camera(void)
{
}

void Camera::BuildViewMatrix()
{
	// Set up projection matrix  
	glMatrixMode(GL_PROJECTION);
	m_lastPerMat = perspective(m_FOV, m_aspectRatio, 0.01f, 1000.0f);
	glLoadMatrixf(&(m_lastPerMat[0][0]));

	// Set up modelview matrix  
	glMatrixMode( GL_MODELVIEW );
	m_lastViewMat = lookAt(m_camPos, m_camTarget, m_camUp);
	glLoadMatrixf(&(m_lastViewMat[0][0]));
}

void Camera::Orbit( float deltaPitch, float deltaYaw, float deltaRad )
{
	// Get the Polar Values
	float thetaOut, phiOut, radOut;
	glm::vec3 deltaPos = m_camPos - m_camTarget;
	mathHelper::deconVector(deltaPos, thetaOut, phiOut, radOut);

	// Change them
	thetaOut += deltaPitch;
	phiOut += deltaYaw;
	radOut += deltaRad;

	// Safe Bounds
	thetaOut = glm::clamp(thetaOut, 0.02f, M_PI * 0.98f);
	mathHelper::wrap(phiOut, 0.0f, M_2PI);
	radOut = glm::clamp(radOut, 0.1f, 500.0f);

	// Update Camera
	mathHelper::buildLook(thetaOut, phiOut, radOut, deltaPos, m_camUp);
	m_camPos = m_camTarget + deltaPos;
}



