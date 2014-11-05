using UnityEngine;
using System.Collections;

public class SwitchAndDitch : MonoBehaviour 
{
	public GameObject respawnPt;
	public Camera orthCam;
	public GameObject gate;

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			GameObject oldScreen = other.transform.parent.gameObject;
			
			other.SendMessage("SetRespawn", respawnPt.transform.position);
			other.transform.parent = transform.parent;
			CamSwitcher.FetchSwitchCam(orthCam);
			gate.gameObject.SetActive(true);
			gameObject.SetActive(false);
			
			oldScreen.SendMessage("StopScreen");
			SendMessageUpwards("StartScreen");
		}
	}
}
