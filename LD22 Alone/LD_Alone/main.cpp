#include "global.h"
#include "include/GL/glew.h"
#include "include/GL/glfw.h"
#include <stdlib.h>
#include <stdio.h>

#include "Camera.h"
#include "AssImpWrapper.h"
#include "PolyHelper.h"

using namespace glm;

bool g_isRunning = true;
Camera g_myCam;
vec3 g_camAction(0,0,0);
vec3 g_planeNormal(0,0,1);
vec3 g_planeCentre(0,0,0);

void errorExit(char errMsg[])
{
	printf(errMsg);
	glfwTerminate();
	exit( EXIT_FAILURE );
}

int callbackCloseWindow()
{
	// Todo :: Anything we want on window closure
	g_isRunning = false;
	return 1;
}

void callbackResizeWindow(int width, int height)
{
	// Setup the Viewport
	glClearColor(0.3f,0.3f,0.7f,1.0f);
	glClearDepth(1.0f);

	g_myCam.m_aspectRatio = (float)width / (float)height;

	glViewport(0,0,width,height);
	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();

	glMatrixMode(GL_MODELVIEW);
}

void callbackKeyboard(  int key, int action )
{
	// GLFW_PRESS or GLFW_RELEASE
	switch(key)
	{ 
	case GLFW_KEY_LEFT:		g_camAction.x = (action == GLFW_PRESS)?+1.0f:0.0f;	break;
	case GLFW_KEY_RIGHT:	g_camAction.x = (action == GLFW_PRESS)?-1.0f:0.0f;	break;
	case GLFW_KEY_UP:		g_camAction.y = (action == GLFW_PRESS)?-1.0f:0.0f;	break;
	case GLFW_KEY_DOWN:		g_camAction.y = (action == GLFW_PRESS)?+1.0f:0.0f;	break;
	case 'G':				g_planeNormal = normalize(g_myCam.m_camPos - g_myCam.m_camTarget); break;
	default:
		printf("Unhandled Key [%d]", key);
	};
}

void callbackMouseButton( int button, int action)
{
	int mX, mY, wX, wY;
	glfwGetMousePos(&mX, &mY);
	glfwGetWindowSize(&wX, &wY);

	float x = (mX / (wX * 1.0f)) - 0.5f;
	float y = 0.5f - (mY / (wY * 1.0f));

	if((button == GLFW_MOUSE_BUTTON_1) && (action == GLFW_PRESS))
	{

	}
}

void drawGround(float extent, float step, float floorHeight)
{
	// Draw our ground grid
	glBegin(GL_LINES);
	float c = -extent;
	float col = 0.0f;
	extent += fmodf(extent, step) + 0.01f;
	while(c < extent)
	{
		col = fabs(c) / extent;
		glColor3f(col, col, col);

		glVertex3f(c, floorHeight,  extent);
		glVertex3f(c, floorHeight, -extent);
		
		glVertex3f( extent, floorHeight, c);
		glVertex3f(-extent, floorHeight, c);
		c += step;
	}
	glEnd();

	glColor3f(1.0f, 1.0f, 1.0f);
}

void drawSphere(float r, int lats, int longs) 
{
	int i, j;
	for(i = 0; i <= lats; i++) {
		float lat0 = M_PI * (-0.5f + (float)(i - 1.0f) / lats);
		float z0  = sinf(lat0);
		float zr0 =  cos(lat0);

		float lat1 = M_PI * (-0.5f + (float) i / lats);
		float z1 = sinf(lat1);
		float zr1 = cosf(lat1);

		glBegin(GL_QUAD_STRIP);
		for(j = 0; j <= longs; j++) 
		{
			float lng = 2.0f * M_PI * (float) (j - 1.0f) / longs;
			float x = cosf(lng);
			float y = sinf(lng);

			glColor3f( ((float)j / (float)longs ), ((float)i / (float)lats ), 0.5f);

			glNormal3f(x * zr0, y * zr0, z0);
			glVertex3f(x * zr0, y * zr0, z0);
			glNormal3f(x * zr1, y * zr1, z1);
			glVertex3f(x * zr1, y * zr1, z1);
		}
		glEnd();
	}
}

void renderCircle( const vec3& planeCentre, const vec3& planeNormal, const float planeSize = 10.0f, const int steps = 16 ) 
{
	float invLength = 1.0f;
	vec3 uOrth;
	vec3 vOrth;

	if (fabs(planeNormal.x) >= fabs(planeNormal.y)) 
	{ 
		invLength = 1.0f / sqrtf(planeNormal.x*planeNormal.x + planeNormal.z*planeNormal.z);
		uOrth.x = -planeNormal.z*invLength;
		uOrth.y = 0.0f;
		uOrth.z = +planeNormal.x*invLength;
		vOrth.x = +planeNormal.y*uOrth.z;
		vOrth.y = +planeNormal.z*uOrth.x - planeNormal.x*uOrth.z;
		vOrth.z = -planeNormal.y*uOrth.x;
	} 
	else 
	{ 
		invLength = 1.0f / sqrtf(planeNormal.y*planeNormal.y + planeNormal.z*planeNormal.z);
		uOrth.x = 0.0f;
		uOrth.y = +planeNormal.z*invLength;
		uOrth.z = -planeNormal.y*invLength;
		vOrth.x = +planeNormal.y*uOrth.z - planeNormal.z*uOrth.y;
		vOrth.y = -planeNormal.x*uOrth.z;
		vOrth.z = +planeNormal.x*uOrth.y;
	}

	float fStep =  M_2PI / (steps+0.0f);
	float cStep =  1.0f / (steps+0.0f);
	vec3* p = new vec3[steps];
	for(int c = 0; c < steps; ++c)
	{
		p[c] = planeCentre + planeSize*(cosf(c*fStep)*uOrth + sinf(c*fStep)*vOrth);
	}

	// Render 
	glBegin(GL_TRIANGLES);
	for(int c = 0; c < steps; ++c)
	{
		glColor3f(0.2f, 0.2f, 0.2f);
		glVertex3fv(&planeCentre[0]);

		glColor3f(cStep * c, 1.0f, 1.0f);
		glVertex3fv(&(p[c][0]));
		glVertex3fv(&(p[(c+1) % steps][0]));

		// Reverse
		glColor3f(0.2f, 0.2f, 0.2f);
		glVertex3fv(&planeCentre[0]);

		glColor3f(1.0f, cStep * c, 1.0f);
		glVertex3fv(&(p[(c+1) % steps][0]));
		glVertex3fv(&(p[c][0]));
	}
	glEnd();
	glColor3f(1.0f, 1.0f, 1.0f);

	delete[] p;
}

//////////////////////////////////////////////////////////////////////////
// PRE-UPDATE
void preUpdate( float deltaTime ) 
{
	g_myCam.Orbit( g_camAction.y * deltaTime, g_camAction.x * deltaTime, 0.0f);
}

//////////////////////////////////////////////////////////////////////////
// Update
void updateLogic( float deltaTime ) 
{
	int mX, mY, wX, wY;
	glfwGetMousePos(&mX, &mY);
	glfwGetWindowSize(&wX, &wY);

	if(glfwGetMouseButton(GLFW_MOUSE_BUTTON_1) == GLFW_PRESS)
	{
		mat4 transMat = inverse(g_myCam.m_lastPerMat * g_myCam.m_lastViewMat);
		vec4 mouseRay(
			(mX / (wX * 0.5f)) - 1.0f,
			1.0f - (mY / (wY * 0.5f)), 
			1.0f, 1.0f);

		mouseRay = transMat * mouseRay;
		mouseRay /= mouseRay.w;

		// Collide
		vec4 res = PolyHelper::intersectSegPlane(g_myCam.m_camPos, g_myCam.m_camPos + vec3(mouseRay), g_planeNormal, g_planeCentre);
		if(fabs(res.w) > 0.01f)
		{
			g_planeCentre = vec3(res);
		}
	}
}

void drawScene( AssImpWrapper &aiWrapper ) 
{
	g_myCam.BuildViewMatrix();

	drawGround(100.0f, 5.0f, 0.0f);

	glEnable( GL_TEXTURE_2D );
	glPushMatrix();
		glColor3f(1.0f, 1.0f, 1.0f);
		glScalef(0.1f, 0.1f, 0.1f);
		aiWrapper.renderAsset("chair");
	glPopMatrix();
	glDisable( GL_TEXTURE_2D );


	renderCircle(g_planeCentre, g_planeNormal, 10.0f, 32);
}

int main(int argc, char* argv[]) 
{
	if (!glfwInit()) { errorExit("Failed to Init");}

	if( !glfwOpenWindow(800,600, 8, 8, 8, 8, 8, 8, GLFW_WINDOW) )
	{
		errorExit("Failed to Create a Window");
	}
	glfwSetWindowTitle("LD48: Alone by Kimau");
	glfwSetWindowSizeCallback(callbackResizeWindow);
	glfwSetWindowCloseCallback(callbackCloseWindow);
	glfwSetMouseButtonCallback(callbackMouseButton);
	glfwSetKeyCallback(callbackKeyboard);

	// Setup
	glShadeModel(GL_SMOOTH);
	glEnable(GL_DEPTH_TEST);
	glClearDepth(1.0f);
	glDepthFunc(GL_LEQUAL);
	glHint(GL_PERSPECTIVE_CORRECTION_HINT, GL_NICEST);
	glEnable(GL_CULL_FACE);

	// Some Rushed Lighting
	// glEnable(GL_LIGHTING);
	// glEnable(GL_LIGHT0);
	// glLightModeli(GL_LIGHT_MODEL_TWO_SIDE, GL_TRUE);

	AssImpWrapper aiWrapper;

	aiWrapper.loadAsset("chair", "Assets/Chair/wingback.3ds");
	unsigned int texId = aiWrapper.loadTexture("Assets/Chair/chair_texture.tga");

	// Setup View
	g_myCam.m_camPos = s_XVec * 50.0f + s_YVec;
	
	// Main loop
	g_isRunning = true;
	double oldTime = glfwGetTime();
	double newTime = glfwGetTime();
	float deltaTime = (float)(newTime - oldTime); 
	while( g_isRunning )
	{
		preUpdate(deltaTime);


		//////////////////////////////////////////////////////////////////////////
		// OpenGL rendering goes here...
		glClear( GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT );
		drawScene(aiWrapper);

		//////////////////////////////////////////////////////////////////////////
		// Swap front and back rendering buffers
		// Also would you believe this updates the Event Pump *facepalm*
		glfwSwapBuffers();
		oldTime = newTime;
		newTime = glfwGetTime();
		deltaTime = (float)(newTime - oldTime); 


		updateLogic(deltaTime);

		// Check if ESC key was pressed or window was closed
		g_isRunning &= !glfwGetKey( GLFW_KEY_ESC ) && glfwGetWindowParam( GLFW_OPENED );
	}

	glfwCloseWindow();
	glfwTerminate();
 
    exit( EXIT_SUCCESS );
}


