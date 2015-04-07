using UnityEngine;
using System.Collections;

public class InGameUI : MonoBehaviour 
{
	
	public void ExitToMenu()
	{
		Application.LoadLevel("front");
	}
}
