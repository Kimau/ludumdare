using UnityEngine;
using System.Collections;

public class ActivateElvator : MonoBehaviour 
{
	public Animation anim;
	public void OnTriggerEnter(Collider col)
	{
		if(col.tag == "Player")
		{
			if(anim.isPlaying == false)
				anim.Play();
		}
	}
}
