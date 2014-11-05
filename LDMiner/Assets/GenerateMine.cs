using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateMine : MonoBehaviour 
{
	public readonly int surfaceWidth = 5;
	public readonly int surfaceHeight = 5;
	
	public GameObject template;
	public MineBlock[] surfaceLevel;
	public FloodFXScript floodFX;
	public int[] resLevels;
	public int fuelLevel;
	public int score;
	
	public GameObject finalScoreObj;
	public GameObject gameUIStuff;
	
	Plane mousePlane;
	int blockingFXCount = 0;
	
	// Use this for initialization
	void Start () 
	{
		finalScoreObj.SetActive(false);
		gameUIStuff.SetActive(true);
		
		// Destroy Children
		for (int i = transform.childCount-1; i  >= 0; i--)
			Destroy(transform.GetChild(i).gameObject);
		
		score = 0;
		fuelLevel = 100;
		resLevels = new int[MineBlock.NOOF_COLOURS];
		for (int i = 0; i < resLevels.Length; i++)
			resLevels[i] = 0;
		
		GenerateNewSurface();
		
		mousePlane = new Plane(new Vector3(0,0,1), transform.position);
	}
	
	void GenerateNewSurface()
	{
		// Logic
		surfaceLevel = new MineBlock[surfaceWidth*surfaceHeight];
		for (int i = 0; i < surfaceLevel.Length; i++) 
		{
			MineBlock newBlock = ScriptableObject.CreateInstance<MineBlock>();
			newBlock.blockType = Random.Range(0, MineBlock.NOOF_COLOURS);
			newBlock.depth = 0;
			
			// Visual Rep			
			newBlock.model = Instantiate(template) as GameObject;
			newBlock.model.name = "block" + i.ToString("D3");
			newBlock.model.transform.parent = transform;
			newBlock.model.transform.localPosition = new Vector3(i % surfaceWidth, i / surfaceWidth, 5.0f);
			
			newBlock.model.renderer.material.SetColor("_Color", MineBlock.MatColours[newBlock.blockType]);
			
			// Setup Data
			surfaceLevel[i] = newBlock;
		}
		
		// Regen every other block using generate rules
		for (int y = 0; y < surfaceHeight; ++y) 
			for (int x = y % 2; x < surfaceWidth; x += 2) 
				RegenBlock(x + y*surfaceWidth);
	}

	void UpdateProbTable(int bID, ref float[] genChance)
	{
		// Don't Sample Mined out blocks
		if(surfaceLevel[bID].minedOut)
			return;
		
		switch(surfaceLevel[bID].blockType)
		{
		case MineBlock.GREEN_BLOCK:
			genChance[MineBlock.GREEN_BLOCK]  += 0.10f;
			genChance[MineBlock.BLUE_BLOCK]   += 0.30f;
			genChance[MineBlock.PURPLE_BLOCK] += 0.15f;
			genChance[MineBlock.RED_BLOCK]    += 0.15f;
			genChance[MineBlock.YELLOW_BLOCK] += 0.30f;
			break;
			
		case MineBlock.BLUE_BLOCK:
			genChance[MineBlock.GREEN_BLOCK]  += 0.30f;
			genChance[MineBlock.BLUE_BLOCK]   += 0.10f;
			genChance[MineBlock.PURPLE_BLOCK] += 0.30f;
			genChance[MineBlock.RED_BLOCK]    += 0.15f;
			genChance[MineBlock.YELLOW_BLOCK] += 0.15f;
			break;
			
		case MineBlock.PURPLE_BLOCK:
			genChance[MineBlock.GREEN_BLOCK]  += 0.15f;
			genChance[MineBlock.BLUE_BLOCK]   += 0.30f;
			genChance[MineBlock.PURPLE_BLOCK] += 0.10f;
			genChance[MineBlock.RED_BLOCK]    += 0.30f;
			genChance[MineBlock.YELLOW_BLOCK] += 0.15f;
			break;
			
		case MineBlock.RED_BLOCK:
			genChance[MineBlock.GREEN_BLOCK]  += 0.15f;
			genChance[MineBlock.BLUE_BLOCK]   += 0.15f;
			genChance[MineBlock.PURPLE_BLOCK] += 0.30f;
			genChance[MineBlock.RED_BLOCK]    += 0.10f;
			genChance[MineBlock.YELLOW_BLOCK] += 0.30f;
			break;
			
		case MineBlock.YELLOW_BLOCK:
			genChance[MineBlock.GREEN_BLOCK]  += 0.30f;
			genChance[MineBlock.BLUE_BLOCK]   += 0.15f;
			genChance[MineBlock.PURPLE_BLOCK] += 0.15f;
			genChance[MineBlock.RED_BLOCK]    += 0.30f;
			genChance[MineBlock.YELLOW_BLOCK] += 0.10f;
			break;
		}
	}

	void GenerateNewBlockID(int blockID)
	{
		// Setup Prob Table
		float[] genChance = new float[MineBlock.NOOF_COLOURS];
		for (int i = 0; i < genChance.Length; i++) 
			genChance[i] = 0.0f;
		
		// Sample Neighbours
		int x = blockID % surfaceWidth;
		int y = blockID / surfaceWidth;
		
		if(x > 0)
			UpdateProbTable(blockID-1, ref genChance);
		if(x < (surfaceWidth-1))
			UpdateProbTable(blockID+1, ref genChance);
		if(y > 0)
			UpdateProbTable(blockID-surfaceWidth, ref genChance);
		if(y < (surfaceHeight-1))
			UpdateProbTable(blockID+surfaceWidth, ref genChance);
		
		float probTotal = 0.0f;
		for (int i = 0; i < genChance.Length; i++) 
			probTotal += genChance[i];
		
		// Random along length of Range then iterrate till in Block
		probTotal = Random.value * probTotal;
		int newTypeID = 0;
		while(probTotal > genChance[newTypeID])
			probTotal -= genChance[newTypeID++];
		
		surfaceLevel[blockID].blockType = newTypeID;
	}
	
	void RegenBlock(int blockID)
	{
		GenerateNewBlockID(blockID);
		
		MineBlock mb = surfaceLevel[blockID];		
		mb.model.renderer.material.SetColor("_Color", MineBlock.MatColours[mb.blockType]);
		mb.model.transform.localPosition = new Vector3(
			mb.model.transform.localPosition.x,
			mb.model.transform.localPosition.y,
			mb.depth * 0.2f + 5.0f);
		mb.minedOut = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(blockingFXCount > 0)
			return;
		
		if(fuelLevel <= 0)
		{
			// Final Score
			score = 0;
			for (int i = 0; i < resLevels.Length; i++)
				score = resLevels[i];
			
			// Display Final Score and Exit Button
			finalScoreObj.SetActive(true);
			gameUIStuff.SetActive(false);
			return;
		}
		
		HitMineBlockTest();		
	}
	
	void FloodBlock(int i, int blockType, int depth)
	{
		if( (surfaceLevel[i].minedOut) || 
			(surfaceLevel[i].blockType != blockType) ||
			(surfaceLevel[i].depth < depth))
			return;
		
		// Flood Mine
		score += 10;
		resLevels[blockType] += 3;					
		surfaceLevel[i].minedOut = true;
		
		// Flood Neighbours
		int x = i % surfaceWidth;
		int y = i / surfaceWidth;
		
		if(x > 0)					FloodBlock(i-1, blockType, surfaceLevel[i].depth);
		if(x < (surfaceWidth-1))	FloodBlock(i+1, blockType, surfaceLevel[i].depth);
		if(y > 0)					FloodBlock(i-surfaceWidth, blockType, surfaceLevel[i].depth);
		if(y < (surfaceHeight-1))	FloodBlock(i+surfaceWidth, blockType, surfaceLevel[i].depth);
		
		// Effect
		StartCoroutine(PlayFloodEffect(i));
		
		// Regen
		surfaceLevel[i].depth += 1;
		RegenBlock(i);   // Might not be the best place to regen
	}
	
	IEnumerator PlayFloodEffect(int i)
	{
		blockingFXCount += 1;
		
		// Gen Particles
		GameObject fxObj = Instantiate(floodFX.gameObject, surfaceLevel[i].model.transform.position + new Vector3(0,0,-5.0f), Quaternion.LookRotation(new Vector3(0,0,-1))) as GameObject;
		
		FloodFXScript fxScript = fxObj.GetComponent<FloodFXScript>();
		fxScript.SetFloodType(surfaceLevel[i].blockType);
		
		// Wait on FX script		
		while(fxScript.isDone == false)
		{
			yield return new WaitForSeconds(0.2f);
		}
		
		Destroy(fxObj);
		
		blockingFXCount -= 1;
	}

	void HitMineBlockTest()
	{
		// Spam for now - should do a single plane cast and UV match but meh
		if(Input.GetMouseButtonUp(0))
		{
			Ray mouseRay = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
			float entryDist = 10000000.0f;
			if(mousePlane.Raycast(mouseRay, out entryDist))
			{
				Vector3 hitPoint = mouseRay.GetPoint(entryDist);
				hitPoint = transform.InverseTransformPoint(hitPoint) - new Vector3(-0.5f, -0.5f, 0.0f);
				
				Debug.Log("Hit Point : " + hitPoint);
				
				if((hitPoint.x < 0) || (hitPoint.x > surfaceWidth) || (hitPoint.y < 0) || (hitPoint.y > surfaceHeight))
					return; // Out of Bounds
				
				int blockI = Mathf.FloorToInt(hitPoint.x) + Mathf.FloorToInt(hitPoint.y) * surfaceWidth;
				
				Debug.Log("Block " + blockI + " at " + surfaceLevel[blockI].model.transform.localPosition);
				
				// Mine Block
				MineBlock mb = surfaceLevel[blockI];
				if(mb.minedOut == false)
				{
					fuelLevel -= 1;
									
					// Flood Fill
					FloodBlock(blockI, mb.blockType, mb.depth);
				}
			}
		}
	}
}
