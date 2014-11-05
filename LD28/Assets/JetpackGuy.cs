using UnityEngine;
using System.Collections;

public class JetpackGuy : MonoBehaviour
{
	public Animator JetpackAnim;

	bool m_isFlying;
	Vector3 m_flyVel;

	// Use this for initialization
	void Start ()
	{
		m_isFlying = false;
		m_flyVel = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Flight Control
		transform.Rotate (
			0.0f, 0.0f,
			Time.deltaTime * ((Input.GetKey (KeyCode.A) ? 20.0f : 0.0f) - (Input.GetKey (KeyCode.D) ? 20.0f : 0.0f)));
		// Fire Control
		m_isFlying = Input.GetKey (KeyCode.W);
		JetpackAnim.SetBool ("Flying", m_isFlying);

		if (m_isFlying)
			m_flyVel += transform.up * Time.deltaTime;
		else
			m_flyVel += 0.1f * Physics.gravity * Time.deltaTime;


		transform.position += m_flyVel * Time.deltaTime;
	}
}
