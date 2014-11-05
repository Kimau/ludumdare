using UnityEngine;
using System.Collections;

public class StealthRoom : MonoBehaviour 
{	
	public void PlayerCaught()
	{
		gameObject.BroadcastMessage("StopLevel", SendMessageOptions.RequireReceiver);
	}
}
