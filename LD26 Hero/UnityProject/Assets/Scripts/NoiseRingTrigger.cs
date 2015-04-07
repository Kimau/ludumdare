using UnityEngine;
using System.Collections;

public class NoiseRingTrigger : MonoBehaviour 
{	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "BadGuy")
		{
			other.SendMessage("Noise", transform.position);
		}
	}
}
