using UnityEngine;
using System.Collections;

public class endScript : MonoBehaviour 
{	
	void OnTriggerExit(Collider col)
	{
		if(col.tag == "Player")
		{
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}
