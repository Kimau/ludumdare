using UnityEngine;
using System.Collections;

public class Toucher : MonoBehaviour 
{
	int touchStage = 0;
	Vector3 lastWorldPos = Vector3.zero;
	Vector3 lastScreenPos = Vector3.zero;
	Vector2 lastViewPos = Vector3.zero;
	RaycastHit lastCol;
	bool worldTouch = false;
	
	public bool TouchUp { get { return (touchStage == 2); } }
	public bool TouchActive { get { return (touchStage == 1); } }
	public int TouchStage { get { return touchStage; } }
	public Vector3 LastViewPos { get { return lastViewPos; } }
	public Vector3 LastScreenPos { get { return lastScreenPos; } }

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(touchStage == 0)
		{
			if(Input.GetButton("Fire1"))
			{
				touchStage = 1;
				ProcessTouchDown();
			}
		}
		else if(touchStage == 1)
		{
			if(Input.GetButton("Fire1") == false)
			{
				touchStage = 2;
				ProcessTouchUp();
			}
			else
			{
				ProcessTouchMove();
			}
		}	
		else
		{
			touchStage = 0;
		}
	}

	void UpdateLast ()
	{
		lastScreenPos = Input.mousePosition;
		lastViewPos = Camera.mainCamera.ScreenToViewportPoint(lastScreenPos);
		Ray touchRay = Camera.mainCamera.ScreenPointToRay(lastScreenPos);		
		worldTouch = Physics.Raycast(touchRay, out lastCol, 1000.0f);
	}

	void ProcessTouchDown()
	{
		// Touch Down
		UpdateLast();
	}

	void ProcessTouchUp()
	{
		// Touch Up
		UpdateLast();
	}

	void ProcessTouchMove()
	{
		// Touch Move
		UpdateLast();
	}
}
