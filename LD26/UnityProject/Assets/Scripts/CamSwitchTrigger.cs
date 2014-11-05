using UnityEngine;
using System.Collections;

public class CamSwitchTrigger : MonoBehaviour 
{
	public Camera newCam;

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			CamSwitcher.FetchSwitchCam(newCam);
		}
	}
}
