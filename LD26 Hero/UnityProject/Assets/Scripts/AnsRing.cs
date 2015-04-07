using UnityEngine;
using System.Collections;

public class AnsRing : MonoBehaviour 
{
	public string answer;
	
	void OnTriggerEnter(Collider col)
	{
		if(col.tag == "Player")
		{
			StartCoroutine(TriggerAns());
		}
	}
	
	IEnumerator TriggerAns()
	{
		for(float x = transform.localScale.x; x > 0.1f; x *= 0.99f)
		{
			transform.localScale = Vector3.one * x;
			yield return null;
		}
		
		gameObject.SendMessageUpwards(answer);
		
	}
}
