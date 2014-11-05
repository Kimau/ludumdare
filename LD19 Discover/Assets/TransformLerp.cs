using UnityEngine;

public class TransformLerp
{
	Vector3[] m_Position;
	Vector3[] m_Scale;
	Quaternion[] m_Quart;
	
	float m_transTime;
	float m_timer;
	
	private TransformLerp () {}
		
	public TransformLerp (float TransTime, Vector3[] movPos, Vector3[] movScale, Quaternion[] movRot)
	{
		m_Position = movPos;
		m_Scale = movScale;
		m_Quart = movRot;
		
		m_transTime = TransTime;
		m_timer = 0.0f;
	}
	
	public void Update(float timeDelta, Transform movingTrans)
	{
		m_timer += timeDelta;
		float perTime = Mathf.Min(1.0f, (m_timer / m_transTime));
		
		movingTrans.localPosition = Vector3.Slerp(m_Position[0], m_Position[1], perTime);
		movingTrans.localScale = Vector3.Slerp(m_Scale[0], m_Scale[1], perTime);
		movingTrans.localRotation = Quaternion.Slerp(m_Quart[0], m_Quart[1], perTime);
	}
	
	public void UpdateTarget(Transform newTarget)
	{
		m_Position[1] = newTarget.localPosition;
		m_Scale[1] = newTarget.localScale;
		m_Quart[1] = newTarget.localRotation;
	}
	
	public bool Active()
	{
		return (m_timer < m_transTime);
	}
}


