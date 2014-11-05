using UnityEngine;
using System.Collections;

public class HideAfter : MonoBehaviour
{

	public float m_timeOut;
	public float m_fadeTime;
	float m_countUp;
	SpriteRenderer m_sr;

	// Use this for initialization
	void Start()
	{
		m_countUp = 0;
		m_sr = GetComponent<SpriteRenderer>();
		m_sr.color = new Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
	}
	
	// Update is called once per frame
	void Update()
	{
		m_countUp += Time.deltaTime;
		if( m_countUp > m_timeOut )
		{
			m_sr.color = new Vector4( 1.0f, 1.0f, 1.0f, 1.0f - ((m_countUp - m_timeOut) / m_fadeTime) );
			if( m_countUp > (m_timeOut + m_fadeTime))
				Destroy( gameObject );
		}
	}
}
