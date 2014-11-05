using UnityEngine;
using System.Collections;

public class ClimberLogic : MonoBehaviour
{
	public bool m_inFlight;
	public bool m_attached;
	public bool m_freefall;
	public Vector3 m_speed;
	public Vector3 m_travelSpeed;
	public float m_afterTouch;
	public GameObject m_rippedOffArms;
	public float m_deadTimer;
	public bool m_carrying;
	public SpriteRenderer m_gameOverMsg;

	Animator m_animControl;
	CircleCollider2D m_cirCollide;
	bool[] m_hitPads;
	Vector2[] m_radPts;

	// Use this for initialization
	void Start()
	{
		m_gameOverMsg.color = Vector4.zero;
		m_carrying = false;
		m_deadTimer = 0.0f;
		m_inFlight = false;
		m_attached = true;
		m_speed = Vector3.zero;
		m_travelSpeed = Vector3.zero;

		m_animControl = GetComponent<Animator>();
		m_cirCollide = GetComponent<CircleCollider2D>();

		float[] edgePt = {
			Mathf.Deg2Rad * (0.0f),
			Mathf.Deg2Rad * (60.0f),
			Mathf.Deg2Rad * (120.0f),
			Mathf.Deg2Rad * (180.0f),
			Mathf.Deg2Rad * (240.0f),
			Mathf.Deg2Rad * (300.0f)
		};
		
		Vector2[] compDir = {
			new Vector2( Mathf.Sin(edgePt[0]), Mathf.Cos(edgePt[0]) ),
			new Vector2( Mathf.Sin(edgePt[1]), Mathf.Cos(edgePt[1]) ),
			new Vector2( Mathf.Sin(edgePt[2]), Mathf.Cos(edgePt[2]) ),
			new Vector2( Mathf.Sin(edgePt[3]), Mathf.Cos(edgePt[3]) ),
			new Vector2( Mathf.Sin(edgePt[4]), Mathf.Cos(edgePt[4]) ),
			new Vector2( Mathf.Sin(edgePt[5]), Mathf.Cos(edgePt[5]) )
		};
		
		// Test Collision List
		m_radPts = new Vector2[] {
			Vector2.zero,
			compDir[0] * m_cirCollide.radius * 0.5f,
			compDir[1] * m_cirCollide.radius * 0.5f,
			compDir[2] * m_cirCollide.radius * 0.5f,
			compDir[3] * m_cirCollide.radius * 0.5f,
			compDir[4] * m_cirCollide.radius * 0.5f,
			compDir[5] * m_cirCollide.radius * 0.5f,
			compDir[0] * m_cirCollide.radius * 1.2f,
			compDir[1] * m_cirCollide.radius * 1.2f,
			compDir[2] * m_cirCollide.radius * 1.2f,
			compDir[3] * m_cirCollide.radius * 1.2f,
			compDir[4] * m_cirCollide.radius * 1.2f,
			compDir[5] * m_cirCollide.radius * 1.2f
		};
	}

	Vector3 UpdateInput()
	{
		Vector3 desiredDir = Vector3.zero;
		if( Input.GetKey( KeyCode.W ) )
			desiredDir.y += 1.2f;
		if( Input.GetKey( KeyCode.S ) )
			desiredDir.y -= 1.5f;
		if( Input.GetKey( KeyCode.D ) )
			desiredDir.x += 1.5f;
		if( Input.GetKey( KeyCode.A ) )
			desiredDir.x -= 1.5f;
		if( Input.GetKey( KeyCode.Space ) )
			desiredDir.z += 1.0f;
		return desiredDir;
	}

	void TryLand()
	{
		m_inFlight = false;
		Vector3 travelDir = Vector3.zero;

		if(CheckColPad(ref travelDir))
			m_attached = true;
	}

	void OnDrawGizmosSelected()
	{
		if(m_radPts == null)
			return;

		m_cirCollide = GetComponent<CircleCollider2D>();
		Vector2 midPt = new Vector2( transform.position.x, transform.position.y ) + m_cirCollide.center;
		for (int i = 0; i < m_radPts.Length; i++)
		{
			Gizmos.color = m_hitPads[i]?Color.red:Color.green;
			Gizmos.DrawSphere(midPt + m_radPts[i], 0.2f);
		}
	}

	bool CheckColPad( ref Vector3 desiredDir )
	{
		Vector2 midPt = new Vector2( transform.position.x, transform.position.y ) + m_cirCollide.center;

		// Test Collision List
		m_hitPads = new bool[m_radPts.Length];
		for (int i = 0; i < m_radPts.Length; i++)
			m_hitPads[i] = Physics2D.OverlapPoint( midPt + m_radPts[i] );

		// Check if we can land
		bool canLand = ((((m_hitPads[ 0 ])?1:0) + 
		                 ((m_hitPads[ 1 ])?1:0) + 
		                 ((m_hitPads[ 2 ])?1:0) + 
		                 ((m_hitPads[ 3 ])?1:0) + 
		                 ((m_hitPads[ 4 ])?1:0) + 
		                 ((m_hitPads[ 5 ])?1:0) + 
		                 ((m_hitPads[ 6 ])?1:0)) >= 7);

		// Travel Directions
		if(desiredDir.z <= 0)
		{
			if( (desiredDir.y > 0) && (!m_hitPads[ 7 ]) )
				desiredDir = new Vector2( desiredDir.x, 0.0f );
			if( (desiredDir.y < 0) && (!m_hitPads[ 10 ]) )
				desiredDir = new Vector2( desiredDir.x, 0.0f );
			if( (desiredDir.x > 0) && (!m_hitPads[ 8 ] || !m_hitPads[ 9 ]) )
				desiredDir = new Vector2( 0.0f, desiredDir.y );
			if( (desiredDir.x < 0) && (!m_hitPads[ 11 ] || !m_hitPads[ 12 ]) )
				desiredDir = new Vector2( 0.0f, desiredDir.y );
		}

		return canLand;
	}

	void UpdateControl()
	{
		Vector3 desiredDir = UpdateInput();
		// Check ColPad
		bool canLand = CheckColPad( ref desiredDir );
		if( m_attached )
		{
			// Attached to Wall and under control
			m_travelSpeed = new Vector3( desiredDir.x, desiredDir.y, 0.0f );
			if( desiredDir.z > 0 )
			{
				m_inFlight = true;
				m_attached = false;
			}

			m_speed = m_travelSpeed;

			// Check for Drop Off
			if(m_carrying && (transform.position.y < 0.7f))
			{
				HostageLogic hl = GetComponentInChildren<HostageLogic>();
				Destroy(hl.gameObject);
				m_carrying = false;
			}
		}
		else
			if( m_inFlight )
			{
				m_speed = m_travelSpeed;
			}
			else
			{
				m_speed += new Vector3( 0.0f, -15.0f, 0.0f ) * Time.deltaTime;
				if( canLand )
				{
					m_attached = true;
				}
			}
	}

	void LoseArms()
	{
		if(!m_inFlight)
			return;

		GameObject arms = Instantiate(m_rippedOffArms) as GameObject;
		arms.transform.position = transform.position + new Vector3(0.0f, 1.16f, 0.0f);
		m_inFlight = false;
		m_speed = new Vector3(0,-1.0f,0.0f);
	}
	
	// Update is called once per frame
	void Update()
	{
		if(transform.position.y < -0.5)
		{
			if(m_speed.y < -0.1)
			{
				m_freefall = true;
				m_speed = Vector2.zero;
				m_animControl.SetTrigger("Splat");
				BroadcastMessage("StartFreefall");
			}

			m_deadTimer += Time.deltaTime;
			m_gameOverMsg.color = new Vector4(1.0f,1.0f,1.0f, m_deadTimer);

			if(m_deadTimer > 8.0f)
			{
				Application.LoadLevel(0);
			}
			return;
		}

		if(m_speed.y < -17.5f)
		{
			m_freefall = true;
			BroadcastMessage("StartFreefall", SendMessageOptions.DontRequireReceiver);
		}

		Vector3 desiredDir = Vector3.one;

		if(!m_freefall)
			UpdateControl();
		else
		{
			// Dont gain speed while losing arms
			if(!m_inFlight)
				m_speed += new Vector3( 0.0f, -15.0f, 0.0f ) * Time.deltaTime;

			if(!m_attached && CheckColPad( ref desiredDir ))
			{
				m_attached = true;
				m_speed = Vector2.zero;
				m_animControl.SetTrigger("Arm Rip");
				m_inFlight = true;
			}
		}

		// Update Position
		transform.position += m_speed * Time.deltaTime;

		// Update Animator
		m_animControl.SetFloat( "TravelX", m_speed.x );
		m_animControl.SetFloat( "TravelY", m_speed.y );
		m_animControl.SetBool( "InAir", !m_attached );
	}
}
