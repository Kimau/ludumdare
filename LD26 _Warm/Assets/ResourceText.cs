using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMesh))]
public class ResourceText : MonoBehaviour 
{	
	TextMesh tMesh;
	GenerateMine gcMine;
	
	public bool fuelOnly = false;
	
	void Awake()
	{
		tMesh = GetComponent<TextMesh>();
		gcMine = GameObject.FindGameObjectWithTag("MineGen").GetComponent<GenerateMine>();
	}
	
	string FloatToHexColour(float inF)
	{
		int xNum = (int)(inF * 255.0f);
		string res = xNum.ToString("X2");
		return res;
	}
	
	void Update()
	{
		if(gcMine == null)
			return;
		
		string newString = "";
		if(fuelOnly)
		{
			newString += "<b>Fuel</b>: " + gcMine.fuelLevel;
		}
		else
		{
			newString += "<b>Resources</b>\n";
			for (int i = 0; i < gcMine.resLevels.Length; i++) 
			{
				newString += "<color=#" + 
					FloatToHexColour(MineBlock.MatColours[i].r) +
					FloatToHexColour(MineBlock.MatColours[i].g) +
					FloatToHexColour(MineBlock.MatColours[i].b) +
						"FF>" + gcMine.resLevels[i].ToString("D3") + "</color> \n";
			}
		}
		
		tMesh.text = newString;
	}
}
