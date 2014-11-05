using UnityEngine;
using System.Collections;

public class StealthPlayer : MonoBehaviour 
{
	Toucher tin;
	Vector3 currDir;
	public float maxSpeed = 10.0f;
	
	bool caught = false;
	bool isRunning = false;
	float noise = 0.0f;
	
	public GameObject[] noiseRings;
	float[] noiseAmt;
	float[] ringTime;
	float timeSinceLastRing;
	float loudestNoiseSinceRing;
	
	void Awake()
	{
		tin = GameObject.FindGameObjectWithTag("GameController").GetComponent<Toucher>();
		noiseAmt = new float[noiseRings.Length];
		ringTime = new float[noiseRings.Length];
		timeSinceLastRing = 0.0f;
		loudestNoiseSinceRing = 0.0f;
	}
	
	void Start()
	{
		caught = false;
	}
	
	void Update() 
	{
		if(caught)
			return;
			
		// Controls
		if(tin.TouchActive)
		{
			Vector3 screenPos = Camera.mainCamera.WorldToScreenPoint(transform.position);
			Vector3 diff = screenPos - tin.LastScreenPos;
			diff.z = 0;
			
			if(tin.LastViewPos.x < 0.1)
				diff.x = +50.0f;
			else if(tin.LastViewPos.x > 0.9)
				diff.x = -50.0f;
			if(tin.LastViewPos.y < 0.1)
				diff.y = +50.0f;
			else if(tin.LastViewPos.y > 0.9)
				diff.y = -50.0f;
			
			float dMag = diff.magnitude;
			if(dMag > 70)
			{
				isRunning = true;
				diff = diff.normalized;
				noise = 1.8f;
			}
			else if(dMag > 5)
			{
				dMag = (dMag / 70) * 0.4f;
				isRunning = false;
				diff = diff.normalized * dMag;
				noise = Mathf.Max(0.3f, dMag);
			}
			else
			{
				noise = 0.0f;
				diff = Vector3.zero;
			}
			
			loudestNoiseSinceRing = Mathf.Max(loudestNoiseSinceRing, noise);			
			
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
			isRunning = false;
			rigidbody.AddForce(-rigidbody.velocity * 2.4f);
		}
		
		timeSinceLastRing += Time.deltaTime;
		
		for(int i = noiseRings.Length-1; i >= 0; --i)
		{
			if( (noiseRings[i].activeSelf == false) && 
				(timeSinceLastRing > 0.25f) &&
				(loudestNoiseSinceRing > 0.01f) &&
				((loudestNoiseSinceRing > 0.5f) || (timeSinceLastRing > 0.5f)) )
			{
				// Make New Noise
				noiseRings[i].SetActive(true);
				noiseRings[i].transform.localScale = Vector3.one * 0.001f;
				noiseRings[i].transform.position = transform.position;
				ringTime[i] = 0.0f;
				noiseAmt[i] = loudestNoiseSinceRing * 1.8f;
				loudestNoiseSinceRing = 0.0f;
				timeSinceLastRing = 0.0f;
			}
			else
			{
				ringTime[i] += Time.deltaTime;
				noiseRings[i].transform.localScale = Vector3.one * Mathf.Lerp(0.001f, noiseAmt[i], ringTime[i]);
				
				if(ringTime[i] > 1.0f)
				{
					noiseRings[i].SetActive(false);					
				}
			}
		}
	}
	
	public void Caught()
	{
		if(caught)
			return;
		
		caught = true;
		StartCoroutine(CapturedSeq());
	}
	
	IEnumerator CapturedSeq()
	{
		SendMessageUpwards("PlayerCaught");
		rigidbody.velocity = Vector3.zero;
		
		Color newColour = renderer.material.GetColor("_FlatColour");
		
		for (float i = 1.0f; i > 0.0f; i -= 0.1f) 
		{
			newColour.a = i;
			renderer.material.SetColor("_FlatColour", newColour);
			yield return new WaitForSeconds(0.1f);
		}
		
		for (int i = 0; i < noiseRings.Length; i++)
			noiseRings[i].SetActive(false);
		
		yield return new WaitForSeconds(1.0f);		
		newColour.a = 1;
		renderer.material.SetColor("_FlatColour", newColour);
		
		// Reload Level
		Application.LoadLevel(Application.loadedLevel);
	}
}
