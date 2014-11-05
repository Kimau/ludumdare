using UnityEngine;
using System.Collections;

public class NextScene : MonoBehaviour 
{
	public string SceneName;
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			Application.LoadLevel(SceneName);
		}
	}
}
