using UnityEngine;
using System.Collections;

public class EndPlayer : MonoBehaviour 
{
	Toucher tin;
	Vector3 currDir;
	public float maxSpeed = 10.0f;
	
	Vector3 startPos;
	Camera followCam;
		
	void Start()
	{
		tin = GameObject.FindGameObjectWithTag("GameController").GetComponent<Toucher>();
		startPos = transform.position;
		followCam = GetComponentInChildren<Camera>();
	}
	
	void Update() 
	{		
		//
		float distFromStart = (transform.position - startPos).sqrMagnitude;
		if(distFromStart < 1000)
			followCam.orthographicSize = Mathf.Lerp(10, 50, distFromStart / 1000);
		else
			followCam.orthographicSize = 50;
			
		// Controls
		if(tin.TouchActive)
		{
			Vector3 screenPos = Camera.mainCamera.WorldToScreenPoint(transform.position);
			Vector3 diff = screenPos - tin.LastScreenPos;
			diff.z = 0;
			
			if(tin.LastViewPos.x < 0.1)
				diff.x = -50.0f;
			else if(tin.LastViewPos.x > 0.9)
				diff.x = +50.0f;
			if(tin.LastViewPos.y < 0.1)
				diff.y = +50.0f;
			else if(tin.LastViewPos.y > 0.9)
				diff.y = -50.0f;
			
			float dMag = diff.magnitude;
			if(dMag > 70)
			{
				diff = diff.normalized;
			}
			else if(dMag > 5)
			{
				dMag = (dMag / 70) * 0.4f;
				diff = diff.normalized * dMag;
			}
			else
			{
				diff = Vector3.zero;
			}
			
			Vector3 res = 
				Camera.mainCamera.transform.up * diff.y * -100.0f +
				Camera.mainCamera.transform.right * diff.x * -100.0f;
			
			if(rigidbody.velocity.magnitude >= maxSpeed)
			{
				float dotRes = Vector3.Dot(res,rigidbody.velocity.normalized);
				if(dotRes > 0)
					res -= rigidbody.velocity.normalized * dotRes;
			}
			
			rigidbody.AddForce(res);
		}
		else
		{
			rigidbody.AddForce(-rigidbody.velocity * 2.4f);
		}
	}
}
