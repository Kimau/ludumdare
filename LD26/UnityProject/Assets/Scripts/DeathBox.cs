using UnityEngine;
using System.Collections;

public class DeathBox : MonoBehaviour 
{
	public GameObject respawnPt;

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			other.SendMessage("LavaDeath");
		}
	}
}
