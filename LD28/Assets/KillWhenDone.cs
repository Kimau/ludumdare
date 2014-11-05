using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class KillWhenDone : MonoBehaviour
{

	ParticleSystem m_ps;

	void Start()
	{	
		m_ps = GetComponent<ParticleSystem>();
	}

	void Update()
	{
		if( m_ps.isPlaying == false )
			Destroy( gameObject );	
	}
}
