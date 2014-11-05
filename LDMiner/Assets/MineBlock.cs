using UnityEngine;
using System;

[System.Serializable]
public class MineBlock : ScriptableObject 
{
	public GameObject model;
	
	public int blockType = 0;
	public int depth     = 0;
	public bool minedOut = false;
	
	public const int GREEN_BLOCK   = 0;
	public const int YELLOW_BLOCK  = 1;
	public const int RED_BLOCK     = 2;
	public const int PURPLE_BLOCK  = 3;
	public const int BLUE_BLOCK    = 4;	
	public const int NOOF_COLOURS  = 5;
	
	public static Color[] MatColours = 
	{
		Color.green,
		Color.yellow,
		Color.red,
		Color.magenta,
		Color.blue
	};
	
	public const int FUEL_BLOCK    = 5;
}
