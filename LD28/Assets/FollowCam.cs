using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{
	public GameObject m_climber;
	public BoxCollider2D m_buildingBox;

	// Use this for initialization
	void Start()
	{
	
	}
	
	// Update is called once per frame
	void Update()
	{
		Vector3 oldPos = transform.position;
		Vector3 targetPos = new Vector3( m_climber.transform.position.x, 
		                                 m_climber.transform.position.y,
		                                 transform.position.z );

		transform.position = Vector3.Lerp( transform.position, targetPos, 0.1f );

		if(
			m_buildingBox.OverlapPoint( camera.ViewportToWorldPoint( new Vector3( 0.2f, 0.5f, 0.0f ) ) ) &&
			m_buildingBox.OverlapPoint( camera.ViewportToWorldPoint( new Vector3( 0.8f, 0.5f, 0.0f ) ) ) == false )
		{
			transform.position = new Vector3( oldPos.x, transform.position.y, transform.position.z );
		}

		if(
			m_buildingBox.OverlapPoint( camera.ViewportToWorldPoint( new Vector3( 0.5f, 0.2f, 0.0f ) ) ) &&
			m_buildingBox.OverlapPoint( camera.ViewportToWorldPoint( new Vector3( 0.5f, 0.8f, 0.0f ) ) ) == false )
		{
			transform.position = new Vector3( transform.position.x, oldPos.y, transform.position.z );
		}

	}
}
