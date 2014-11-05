using UnityEngine;
using System.Collections;

public class PuzzleCamera : MonoBehaviour 
{
	public float m_dist;
	float m_elevation;
	float m_angle;
	GameObject m_lookAt;
	
	TransformLerp m_MovingTrans;
	
	// Use this for initialization
	void Start () 
	{
		transform.localPosition = new Vector3(m_dist,0,0);
		transform.LookAt(Vector3.zero);
		
		m_elevation = -47.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_dist += Input.GetAxis("CamScrollZoom") * 0.2f; 
		m_elevation -= Input.GetAxis("CamScrollElv") * 0.2f;
		m_angle -= Input.GetAxis("CamScrollRot") * 0.8f;
		
		m_dist = Mathf.Clamp(m_dist, 10.0f, 100.0f);
		
		if(m_lookAt != null)
		{			
			transform.localPosition = 
				m_lookAt.transform.position +
				Quaternion.Euler(m_elevation, m_angle, 0) * new Vector3(0,0,m_dist);
			transform.LookAt(m_lookAt.transform.position);
			
			if(m_MovingTrans != null)
			{
				m_MovingTrans.UpdateTarget(transform);
				m_MovingTrans.Update(Time.deltaTime, transform);
			}
		}
		else
		{
			transform.localPosition = Quaternion.Euler(m_elevation, m_angle, 0) * new Vector3(0,0,m_dist);
			transform.LookAt(Vector3.zero);
		}
	}
	
	public void SelectObj(GameObject gObj)
	{
		m_lookAt = gObj;
		
		m_MovingTrans = new TransformLerp( 1.0f,
			 	new Vector3[]{transform.localPosition, transform.localPosition},
				new Vector3[]{transform.localScale, transform.localScale},
				new Quaternion[]{transform.localRotation, transform.localRotation} );
	}
}
