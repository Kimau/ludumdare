#ifndef ASSIMP_WRAPPER
#define ASSIMP_WRAPPER

class AssImpWrapper
{
public:
	AssImpWrapper(void);
	~AssImpWrapper(void);

	void renderAsset(const char* keyName);
	bool hasKey(const char* keyName);
	void loadAsset(const char* keyName, const char* filepath);
	void freeAsset(const char* keyName, bool recurse = false);
	unsigned int loadTexture(const char* TexName);

private:
	static const int m_MaxKeyLength = 256;
	struct sNode 
	{
		char			keyName[m_MaxKeyLength];
		const struct aiScene*	data;
		sNode*			nextNode;
		unsigned int	glList;

		sNode(const char* newName);
	};

	sNode* findKey(const char* keyName);
	void freeAsset(sNode* node, bool recurse = false);
	void buildGlList(sNode* node);
	void recursive_render (const struct aiScene* sc, const struct aiNode* nd);

	void mapAiColorToGlColor(const struct aiMaterial* mtl, const char* aiKey, unsigned int glKey);
	void apply_material(const struct aiMaterial* mtl);

	

	sNode*	m_firstNode;
};

#endif