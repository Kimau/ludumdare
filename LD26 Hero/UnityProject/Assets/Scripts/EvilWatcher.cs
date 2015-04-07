using UnityEngine;
using System.Collections;

public class EvilWatcher : MonoBehaviour 
{
	public void FoundPlayer(Vector3 playerPos)
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		
		player.SendMessage("Caught");
	}
	
	public void StopLevel()
	{
		animation.Stop();
	}
}
