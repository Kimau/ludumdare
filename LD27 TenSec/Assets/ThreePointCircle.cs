using UnityEngine;
using System.Collections;

public class ThreePointCircle : MonoBehaviour {
	
	int m_pointCount = 0;
	Vector2[] m_points;

	void Start () 
	{
		m_points = new Vector2[3];
	}
	
	void Update () 
	{
		if(Input.GetMouseButtonUp(0))
		{
			m_points[m_pointCount++] = new Vector2(Input.mousePosition.x, Input.mousePosition.y);			
			if(m_pointCount >= 3)
			{
				//
				m_pointCount = 0;
			}
		}
	}
	
	void OnDrawGizmos()
	{		
		if((m_points == null) || (m_points.Length < 3))
			return;
		
		Gizmos.color = Color.white;		
		Gizmos.DrawLine(ToWorld(m_points[0]), ToWorld(m_points[1]));
		Gizmos.DrawLine(ToWorld(m_points[0]), ToWorld(m_points[2]));
		Gizmos.DrawLine(ToWorld(m_points[1]), ToWorld(m_points[2]));
		
		Gizmos.color = Color.green;
		Gizmos.DrawLine(ToWorld(m_points[0]), ToWorld(Input.mousePosition));
		
		Vector2 msPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		int x = 1;
		
		GizmoMouseLine (msPos, 1,2);
		
		// Circle
		float cirRad;
		Vector2 cirMid;
		MathUtils.CalcCircle(m_points, out cirMid, out cirRad);
	}

	void GizmoMouseLine (Vector2 msPos, int x, int y)
	{
		Vector2 res;
		bool inter = MathUtils.IsIntersecting(m_points[0], msPos, m_points[x], m_points[y], out res);
		Gizmos.color = inter?Color.red:Color.green;
		Gizmos.DrawSphere(ToWorld(res), 0.5f);
	}
	
	Vector3 ToWorld(Vector2 pt)
	{
		return Camera.main.ScreenToWorldPoint(new Vector3(pt.x, pt.y, 10.0f));
	}
	
	// Utils
}
