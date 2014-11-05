using UnityEngine;
using System.Collections;

public class HostageLogic : MonoBehaviour
{
	public bool m_onBack;
	public bool m_inFreeFall;
	public float m_screamTime;
	public float m_fallSpeed;

	ClimberLogic m_player;
	Animator m_animCon;

	// Use this for initialization
	void Start()
	{
		m_onBack = false;
		m_inFreeFall = false;
		m_fallSpeed = 0.0f;
		m_animCon = GetComponent<Animator>();
		m_player = GameObject.FindGameObjectWithTag( "Player" ).GetComponent<ClimberLogic>();
	}
	
	// Update is called once per frame
	void Update()
	{

		if(m_inFreeFall)
		{
			m_fallSpeed -= 23.0f * Time.deltaTime;
			transform.position -= Vector3.down * m_fallSpeed * Time.deltaTime;

			if(transform.position.y < 0.5)
				Destroy(gameObject);
		}
		else if(m_onBack)
		{
		}
		else
		{
			m_screamTime -= Time.deltaTime;
			if( m_screamTime < 0.0f )
			{
				m_animCon.SetTrigger( "Scream" );
				m_screamTime = Random.value * 5.0f + 1.0f;
			}

			if((m_player) && 
			   (m_player.m_carrying == false) &&
			   (m_player.m_attached) &&
			   (collider2D.OverlapPoint(m_player.transform.position)))
			{
				m_player.m_carrying = true;
				m_onBack = true;
				Destroy(GetComponent<BoxCollider2D>());
				Destroy(GetComponent<Rigidbody2D>());

				m_animCon.SetTrigger( "Back" );
				transform.parent = m_player.transform;
				transform.localPosition = new Vector3(0.1f,-0.8f,0.0f);
			}
		}
	}

	public void StartFreefall()
	{
		m_inFreeFall = true;
		transform.parent = null;
	}

}
