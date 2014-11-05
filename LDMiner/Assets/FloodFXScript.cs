using UnityEngine;
using System.Collections;

public class FloodFXScript : MonoBehaviour 
{
	public float fxSpeed = 1.0f;
	
	public bool isDone { get { return !(pSys.isPlaying); } }
	
	ParticleSystem pSys;	
	
	void Awake()
	{
		pSys = GetComponent<ParticleSystem>();
		pSys.playbackSpeed = fxSpeed;
	}
	
	public void SetFloodType(int blockType)
	{
		pSys.startColor = MineBlock.MatColours[blockType];
	}
}
