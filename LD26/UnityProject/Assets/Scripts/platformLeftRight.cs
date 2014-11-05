using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class platformLeftRight : MonoBehaviour 
{
	public float MaxPlayVelocity;
	public bool OpeningScreen;
	
	Toucher tin;
	
	SphereCollider spCol;
	
	bool dying = false;
	bool isJumping = false;
	bool onGround = true;
	int moving = 0;
	Vector3 respawnPt;
	float timeSinceLastJump = 0.0f;
	bool groundGuarantee = false;
		
	void Awake()
	{
		tin = GameObject.FindGameObjectWithTag("GameController").GetComponent<Toucher>();
		spCol = GetComponent<SphereCollider>();
	}
	
	void Start()
	{
		moving = 0;
		dying = false;
		respawnPt = transform.position;
	}
	
	public void Respawn()
	{
		moving = 0;
		dying = false;
		transform.position = respawnPt;
		rigidbody.isKinematic = false;
		rigidbody.velocity = Vector3.zero;
		
		SendMessageUpwards("StartScreen");
	}
		
	void Update ()
	{
		if(dying)
			return;
		
		Vector3 playScreenPos = Camera.mainCamera.WorldToScreenPoint(transform.position);
		
		// Update Player Controls
		if(tin.TouchActive)
		{			
			Vector3 scDiff = tin.LastScreenPos - playScreenPos;
			if(tin.LastViewPos.x < 0.1)
				scDiff.x = -50.0f;
			else if(tin.LastViewPos.x > 0.9)
				scDiff.x = +50.0f;
			
			if(Mathf.Abs(scDiff.x) > 10.0f)
				moving = (int)Mathf.Sign(scDiff.x);
			else
				moving = 0;
			
			
			// Jump
			if(scDiff.y > 30.0f)
			{				
				if(isJumping)
					isJumping = false;
				else if(onGround && (tin.TouchActive || tin.TouchUp))
				{
					isJumping = true;
					onGround = false;
				}
			}
		}
		else
		{
			isJumping = false;
			moving = 0;
		}
		
		// Ground Sanity Check
		HingeJoint hj = GetComponent<HingeJoint>();
		
		if(groundGuarantee)
		{
			onGround = true;
			groundGuarantee = false;
		}
		else
		{
			onGround = (Mathf.Abs(Vector3.Dot(rigidbody.velocity, Physics.gravity)) < 0.3f);			
			if( (onGround == false) && 
				(hj == null) && 
				(Physics.Raycast(transform.position, Camera.mainCamera.transform.up * -1.0f, 3.0f)) )
			{
				onGround = true;
			}
		}

		// Let Go
		if(tin.TouchUp && hj)
		{
			Object.Destroy(hj);
		}
		
		// Opening Hack
		if(OpeningScreen)
		{
			moving = Mathf.Max(0,moving);
			isJumping = false;
		}
		
		// Moving
		if(onGround)
		{
			timeSinceLastJump += Time.deltaTime;
			
			if(moving == 0)
			{
				rigidbody.AddForce(Camera.mainCamera.transform.right * 
					Vector3.Dot(Camera.mainCamera.transform.right, rigidbody.velocity) * -1.0f);
			}
			else
			{
			
				float speedMult = MaxPlayVelocity;
				if(rigidbody.velocity.magnitude < 1.0f)
					speedMult *= 10.0f;
				
				Vector3 moveVel = Camera.mainCamera.transform.right * moving * speedMult;
				if( (Vector3.Dot(moveVel, rigidbody.velocity) < 0) ||
					(rigidbody.velocity.magnitude < MaxPlayVelocity))
				{
					rigidbody.AddForce(Camera.mainCamera.transform.right * moving * speedMult);
				}
				
				if(isJumping && (timeSinceLastJump > 0))
				{
					timeSinceLastJump = -0.2f;
					rigidbody.AddForce(Camera.mainCamera.transform.up * 1700.0f);
				}
			}
		}
		else
		{
			rigidbody.AddForce(Camera.mainCamera.transform.up * -20.0f);
		}
	}
	
	public void SetRespawn(Vector3 position)
	{
		respawnPt = position;
	}
	
	public void LavaDeath()
	{
		dying = true;
		StartCoroutine(PlayLavaDeath());
	}
	
	IEnumerator PlayLavaDeath()
	{
		Color newColour = renderer.material.GetColor("_FlatColour");
		rigidbody.isKinematic = true;
		
		Vector3 oldPt = transform.position;
		Vector3 destPt = transform.position - (Camera.mainCamera.transform.up * 5.0f);
		
		for (float i = 1.0f; i > 0.0f; i -= Time.deltaTime) {
			newColour.a = i;
			renderer.material.SetColor("_FlatColour", newColour);
			transform.position = Vector3.Lerp(destPt, oldPt, i);
			yield return null;
		}
		
		yield return new WaitForSeconds(1.0f);		
		newColour.a = 1;
		renderer.material.SetColor("_FlatColour", newColour);
		
		Respawn();
	}
	
	void OnCollisionEnter(Collision col)
	{
		HingeJoint hj = gameObject.GetComponent<HingeJoint>();
		if(hj != null)
			return;
		
		if(col.collider.tag == "Grabbable")
		{
			if(tin.TouchActive)
			{
				hj = gameObject.AddComponent<HingeJoint>();
				hj.connectedBody = col.rigidbody;
				hj.anchor = transform.InverseTransformPoint(col.contacts[0].point);
				hj.axis = Vector3.zero;
			}
		}
		else if(col.collider.tag == "SolidGround")
		{
			groundGuarantee = true;
		}
	}
	
	void OnCollisionStay(Collision col)
	{
		if(col.collider.tag == "SolidGround")
		{
			groundGuarantee = true;
		}
	}
}
