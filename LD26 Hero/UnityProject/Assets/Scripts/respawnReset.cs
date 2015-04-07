using UnityEngine;
using System.Collections;

public class respawnReset : MonoBehaviour 
{
	public GameObject[] resetObjs;
	
	Vector3[] origPosition;
	Quaternion[] origRot;
	
	// Use this for initialization
	void Start () 
	{
		origPosition = new Vector3[resetObjs.Length];
		origRot = new Quaternion[resetObjs.Length];
		
		for(int i = 0; i < resetObjs.Length; i++)
		{
			origPosition[i] = resetObjs[i].transform.position;
			origRot[i] = resetObjs[i].transform.rotation;			
		}
	}
		
	public void StartScreen()
	{
		for(int i = 0; i < resetObjs.Length; i++)
		{
			Rigidbody rBod = resetObjs[i].rigidbody;
			if((rBod) && (rBod.isKinematic == false))
			{
				rBod.MovePosition(origPosition[i]);
				rBod.MoveRotation(origRot[i]);
				
				rBod.velocity = Vector3.zero;
				rBod.angularVelocity = Vector3.zero;				
			}
			else
			{
				resetObjs[i].transform.position = origPosition[i];
				resetObjs[i].transform.rotation = origRot[i];
			}
		}
	}
	
	public void StopScreen()
	{
		gameObject.SetActive(false);
	}
}
