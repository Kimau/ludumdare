using UnityEngine;
using System.Collections;

public class PanningCam : MonoBehaviour 
{
	public GameObject target;
	public Vector3 deadZone;
	public float predictFactor = 1.0f;
	
	Vector3 targetLastFrame;
	Vector3 targetPos;
	float camFollowTime = -1.0f;
	Vector3 camOffset = new Vector3(0, 0.2f, 0);
	

	Vector3 WorldToViewNorm (Vector3 trackForwadPos)
	{
		Vector3 tarViewPt = camera.WorldToViewportPoint(trackForwadPos) - new Vector3(0.5f, 0.5f, 0.0f);
		tarViewPt.z = 0.0f;	
		tarViewPt *= 2.0f;
		
		return tarViewPt;
	}
	
	Vector3 NormedViewToWorld(Vector3 normedView)
	{
		return (normedView * 0.5f) + new Vector3(0.5f, 0.5f, 0.0f);
	}
			
	void Update()
	{
		Vector3 targetVel = ((target.transform.position - targetLastFrame) / Time.deltaTime) * predictFactor;
		Vector3 trackForwadPos = target.transform.position + ((target.transform.position - targetLastFrame) / Time.deltaTime) * predictFactor;
		Vector3 tarViewPt = WorldToViewNorm(trackForwadPos);
		Vector3 tarTrueViewPt = WorldToViewNorm(target.transform.position);
		
		if((Mathf.Abs(tarViewPt.x) > deadZone.x) || (Mathf.Abs(tarViewPt.y) > deadZone.y))
		{
			targetPos = camera.ViewportToWorldPoint(NormedViewToWorld(tarViewPt) + camOffset);
			camFollowTime = 1.0f;
		}
		else
		{
			Vector3 trueTarget = camera.ViewportToWorldPoint(NormedViewToWorld(tarTrueViewPt) + camOffset);
			targetPos = Vector3.Lerp(targetPos, trueTarget, 0.1f);
		}
		
		targetLastFrame = target.transform.position;
	}
	
	void FixedUpdate()
	{
		if(camFollowTime > 0.0f)
		{
			camFollowTime -= Time.fixedDeltaTime;
			transform.position = Vector3.Lerp(targetPos, transform.position, camFollowTime);
		}
	}
	
}
