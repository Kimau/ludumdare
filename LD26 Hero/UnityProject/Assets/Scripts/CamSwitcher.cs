using UnityEngine;
using System.Collections;

public class CamSwitcher : MonoBehaviour 
{
	public Camera[] camList;
	
	public void Start()
	{
		camList = GetComponentsInChildren<Camera>(true);
		SwitchCamera(Camera.mainCamera);
	}
	
	public static void FetchSwitchCam(Camera newCam)
	{
		CamSwitcher cs = GameObject.FindGameObjectWithTag("CamSwitcher").GetComponent<CamSwitcher>();
		if(cs)
			cs.SwitchCamera(newCam);
	}
	
	public void SwitchCamera(Camera newCam)
	{
		
		
		for (int i = 0; i < camList.Length; i++) 
		{
			camList[i].tag = "Untagged";
			camList[i].gameObject.SetActive(false);
		}
		
		newCam.tag = "MainCamera";
		newCam.gameObject.SetActive(true);
	}
}
