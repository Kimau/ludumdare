#include "global.h"
#include "AssImpWrapper.h"

// Open Assest Importer
#include "assImp/assimp.h"
#include "assImp/aiPostProcess.h"
#include "assImp/aiScene.h"

// GL & Game Stuff
#include "include/GL/glew.h"
#include "include/GL/glfw.h"

AssImpWrapper::sNode::sNode( const char* newName ) : data(nullptr), nextNode(nullptr), glList(0)
{
	strncpy_s(keyName, newName, m_MaxKeyLength);
}



AssImpWrapper::AssImpWrapper(void)
{
	m_firstNode = nullptr;
}


AssImpWrapper::~AssImpWrapper(void)
{
	if(m_firstNode != nullptr)
	{
		auto iPtr = m_firstNode->nextNode;
		delete m_firstNode;

		while(iPtr != nullptr)
		{
			m_firstNode = iPtr->nextNode;
			iPtr = m_firstNode->nextNode;
			delete m_firstNode;
		}
	}
}

bool AssImpWrapper::hasKey( const char* keyName )
{
	return (findKey(keyName) != nullptr);
}

AssImpWrapper::sNode* AssImpWrapper::findKey( const char* keyName )
{
	if(m_firstNode == nullptr)
		return nullptr;

	auto iPtr = m_firstNode;
	while(iPtr != nullptr)
	{
		if(strcmp(iPtr->keyName, keyName) == 0)
		{
			return iPtr;
		}

		iPtr = iPtr->nextNode;
	}

	return nullptr;
}

void AssImpWrapper::loadAsset( const char* keyName, const char* filepath )
{
	if(hasKey(keyName))
	{
		POW2_ASSERT_FAIL("Key Already Exsist");
		return;
	}

	sNode* newNode = new sNode(keyName);
	
	newNode->data = aiImportFile(filepath, 
		aiProcess_CalcTangentSpace      |
		aiProcess_GenNormals			|
		aiProcess_JoinIdenticalVertices |
		aiProcess_Triangulate			|
		aiProcess_GenUVCoords           |
		aiProcess_SortByPType);

	newNode->nextNode = nullptr;

	buildGlList(newNode);

	if(m_firstNode == nullptr)
	{
		m_firstNode = newNode;
	}
	else
	{
		auto iPtr = m_firstNode;
		while(iPtr->nextNode != nullptr)
		{
			iPtr = iPtr->nextNode;
		}

		iPtr->nextNode = newNode;
	}
}

void AssImpWrapper::freeAsset( const char* keyName, bool recurse /*= false*/ )
{
	sNode* foundResult = nullptr;

	if(keyName == nullptr)
	{
		foundResult = m_firstNode;
	}
	else
	{
		foundResult = findKey(keyName);
	}

	POW2_ASSERT_MSG(foundResult != nullptr, "Cannot find Key");
	freeAsset(foundResult, recurse);
}

void AssImpWrapper::freeAsset( sNode* node, bool recurse /*= false*/ )
{
	// Find 
	while(node == nullptr)
	{
		aiReleaseImport(node->data);

		if(recurse)
		{
			freeAsset(node->nextNode);
		}

		delete node;
	}
}

void AssImpWrapper::buildGlList( sNode* node )
{
	node->glList = glGenLists(1);
	glNewList(node->glList, GL_COMPILE);
	recursive_render(node->data, node->data->mRootNode);
	glEndList();
}

void AssImpWrapper::recursive_render( const struct aiScene* sc, const struct aiNode* nd )
{
	unsigned int i;
	unsigned int n = 0, t;
	struct aiMatrix4x4 m = nd->mTransformation;

	// update transform
	aiTransposeMatrix4(&m);
	glPushMatrix();
	glMultMatrixf((float*)&m);

	printf_s("%s >> Number of Meshes: %d \n", nd->mName.data, nd->mNumMeshes);

	// draw all meshes assigned to this node
	for (; n < nd->mNumMeshes; ++n) {
		const struct aiMesh* mesh = sc->mMeshes[nd->mMeshes[n]];
		// apply_material(sc->mMaterials[mesh->mMaterialIndex]);

		for (t = 0; t < mesh->mNumFaces; ++t) {
			const struct aiFace* face = &mesh->mFaces[t];
			GLenum face_mode;

			switch(face->mNumIndices) {
			case 1: face_mode = GL_POINTS; break;
			case 2: face_mode = GL_LINES; break;
			case 3: face_mode = GL_TRIANGLES; break;
			default: face_mode = GL_POLYGON; break;
			}

			glBegin(face_mode);

			for(i = 0; i < face->mNumIndices; i++) {
				int index = face->mIndices[i];
				if(mesh->mColors[0] != NULL)
					glColor4fv((GLfloat*)&mesh->mColors[0][index]);
				if(mesh->mNormals != NULL) 
					glNormal3fv(&mesh->mNormals[index].x);
				if(mesh->mTextureCoords != NULL) 
					glTexCoord2fv(&mesh->mTextureCoords[0][index].x);
				glVertex3fv(&mesh->mVertices[index].x);
			}

			glEnd();
		}

	}

	// draw all children
	for (n = 0; n < nd->mNumChildren; ++n) {
		recursive_render(sc, nd->mChildren[n]);
	}

	glPopMatrix();
}

void AssImpWrapper::renderAsset( const char* keyName )
{
	sNode* fNode = findKey(keyName);

	if(fNode !=nullptr)
	{
		glCallList(fNode->glList);
	}
}

void AssImpWrapper::mapAiColorToGlColor(const aiMaterial* mtl, const char* aiKey, unsigned int glKey)
{
	float col[4] = {0.8f, 0.8f, 0.8f, 1.0f};
	aiColor4D aic;

	if(AI_SUCCESS != aiGetMaterialColor(mtl, aiKey, 0, 0, &aic))
	{
		col[0] = aic.r;
		col[1] = aic.g;
		col[2] = aic.b;
		col[3] = aic.a;
	}

	glMaterialfv(GL_FRONT_AND_BACK, glKey, col);
}


void AssImpWrapper::apply_material(const struct aiMaterial* mtl)
{
	mapAiColorToGlColor(mtl, "$clr.diffuse", GL_DIFFUSE);
	mapAiColorToGlColor(mtl, "$clr.specular", GL_SPECULAR);
	mapAiColorToGlColor(mtl, "$clr.ambient", GL_AMBIENT);
	mapAiColorToGlColor(mtl, "$clr.emissive", GL_EMISSION);
}

unsigned int AssImpWrapper::loadTexture( const char* TexName )
{
	unsigned int Texture;

	glGenTextures(1,&Texture);
	glBindTexture(GL_TEXTURE_2D,Texture);

	if(glfwLoadTexture2D(TexName, GLFW_BUILD_MIPMAPS_BIT))
	{ 
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
		glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MAG_FILTER,GL_LINEAR);
		return Texture;
	}


	glDeleteTextures(1, &Texture);
	return -1;
}

