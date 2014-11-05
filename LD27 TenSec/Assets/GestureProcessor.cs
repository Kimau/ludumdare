using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestureProcessor : MonoBehaviour {
	
	enum GestureType {
		Ges_Line,
		Ges_Circle,		// CW
		Ges_CircleAnti,	// CCW
		Ges_Spiral,		// CW
		Ges_SpiralAnti,	// CCW
	};
	
	class gPoint {
		public Vector3 m_pos;
		public float m_time;
		
		public gPoint(Vector3 p, float t)
		{
			m_pos = p;
			m_time = t;
		}
	};
	
	class GesturePart {
		float m_quality;
		Vector3 m_pos;
		Vector3 m_posB;
		GestureType m_gType;
		
		public GestureType GType { get { return m_gType; } }
		public Vector3 Center    { get { return m_pos; } }
		public Vector3 LineStart { get { return m_pos; } }
		public Vector3 LineEnd   { get { return m_posB; } }
		public float Radius      { get { return m_posB.x; } }
		public float CircleComp  { get { return m_posB.y; } }
		public float LineLength  { get { return m_posB.magnitude; } }
		public float Quality     { get { return m_quality; } }
		
		public static GesturePart NewLine(float quality, Vector3 startPos, Vector3 endPos)
		{
			GesturePart gp = new GestureProcessor.GesturePart();
			gp.m_gType = GestureProcessor.GestureType.Ges_Line;
			gp.m_pos = startPos;
			gp.m_posB = endPos;
			gp.m_quality = quality;
			
			return gp;
		}
		
		public static GesturePart NewCircle(float quality, Vector3 centerPos, float radius, float angComplete, bool clockwise)
		{
			GesturePart gp = new GestureProcessor.GesturePart();
			if(clockwise)
				gp.m_gType = GestureProcessor.GestureType.Ges_Circle;
			else
				gp.m_gType = GestureProcessor.GestureType.Ges_CircleAnti;
			
			gp.m_pos = centerPos;
			gp.m_posB = new Vector3(radius, angComplete, 0.0f);
			gp.m_quality = quality;
			
			return gp;
		}
	};
	
	public float m_rawSegLength = 10.0f;
	public float m_lineTol = 0.8f;
	public float m_drawTime = 0.0f;
	public bool m_isDrawing = false;
	
	List<gPoint> m_rawPointList;
	List<GesturePart> m_gesList;
	
	bool m_possLine;
	bool m_possCircle;
	bool m_possSpiral;
	
	Vector3 MousePoint { get { return Input.mousePosition + new Vector3(0,0,10); }}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_isDrawing)
		{
			UpdateDrawing();
		}
		else if(Input.GetMouseButtonDown(0))
		{
			StartGesture();
		}
	}

	void UpdateDrawing ()
	{
		Vector3 mp = MousePoint;
		m_drawTime += Time.deltaTime;
		
		if(Input.GetMouseButtonUp(0))
		{
			StopGesture();
			if(m_possLine)
				AddLine();			
			return;
		}
		
		// Capture Raw Point
		if((mp - m_rawPointList[m_rawPointList.Count-1].m_pos).magnitude > m_rawSegLength)
			MarkPoint();
		
		if((m_possLine) && (CheckLine(mp) == false))
			AddLine();			
		
		//
		if(m_possLine == false)
			StartGesturePart();
	}

	bool CheckLine(Vector3 mp)
	{
		// Check for straight line
		if(m_rawPointList.Count > 5)
		{
			int earlyMid = Mathf.RoundToInt(m_rawPointList.Count*0.3f);
			int laterMid = Mathf.RoundToInt(m_rawPointList.Count*0.6f);
			
			Vector3[] lineList = 
			{
				(m_rawPointList[4].m_pos - m_rawPointList[0].m_pos).normalized,
				(m_rawPointList[earlyMid].m_pos - m_rawPointList[0].m_pos).normalized,
				(m_rawPointList[laterMid].m_pos - m_rawPointList[earlyMid].m_pos).normalized,
				(mp - m_rawPointList[laterMid].m_pos).normalized,
				(mp - m_rawPointList[m_rawPointList.Count-2].m_pos).normalized
			};
			
			// If we are far away enough
			Vector3 lastBit = (mp - m_rawPointList[m_rawPointList.Count-1].m_pos);
			if(lastBit.magnitude > (m_rawSegLength*0.3f))
				lineList[3] = lastBit.normalized;
			
			// 			
			for(int i=0; i<lineList.Length; ++i)
			{
				for(int j=i+1; j<lineList.Length; ++j)
				{
					if(Vector3.Dot(lineList[i],lineList[j]) < m_lineTol)
					{
						MarkPoint();
						m_possLine = false;
						return false;
					}
				}
			}
			
			//
		}
		
		return m_possLine;
	}
	
	float LineQualityTest(int startPt, int endPt)
	{
		// Measure Quality
		float quality = 1.0f;
		Vector3 startPos = m_rawPointList[startPt].m_pos;
		Vector3 endPos = m_rawPointList[endPt].m_pos;
		Vector3 dir = (endPos - startPos).normalized;
		
		for(int p=m_rawPointList.Count-2; p>0; --p)
			quality -= (1.0f - Vector3.Dot((m_rawPointList[p].m_pos - startPos).normalized, dir));
		
		return quality;
	}

	void AddLine()
	{
		int lastPt = m_rawPointList.Count;
		float quality = LineQualityTest(0, lastPt-1);
		
		if(quality < 0.6f)
		{
			Debug.Log("Bad Line");
			return;
		}		
				
		m_gesList.Add(
			GesturePart.NewLine(
				quality, 
				m_rawPointList[0].m_pos,
				m_rawPointList[lastPt-1].m_pos));
		
		StartGesturePart();
	}
	
	void OnDrawGizmos()
	{
		if((m_rawPointList == null) || (m_rawPointList.Count < 1))
			return;
		
		Color startColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
		Color endColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
		
		// Draw Current Line
		Gizmos.color = Color.Lerp(startColor, endColor, 0.0f);
		Vector3 prevWPos = Camera.main.ScreenToWorldPoint(m_rawPointList[0].m_pos);
		Vector3 wPos;

		for(int i=1; i < m_rawPointList.Count; ++i)
		{
			Gizmos.color = Color.Lerp(startColor, endColor, (float)i / (float)m_rawPointList.Count);
			
			wPos = Camera.main.ScreenToWorldPoint(m_rawPointList[i].m_pos);
			Gizmos.DrawLine(prevWPos, wPos);
			prevWPos = wPos;
		}
		
		if(m_isDrawing)
		{
			Gizmos.color = Color.Lerp(startColor, endColor, 1.0f);
			Gizmos.DrawLine(prevWPos, Camera.main.ScreenToWorldPoint(MousePoint));
		}
		
		Gizmos.color = Color.white;
		for(int i=0; i<m_gesList.Count; ++i)
		{
			switch(m_gesList[i].GType)
			{
			case GestureType.Ges_Line:
				Gizmos.color = Color.white * Mathf.Pow(m_gesList[i].Quality, 4.0f);
				Gizmos.DrawLine(
					Camera.main.ScreenToWorldPoint(m_gesList[i].LineStart), 
					Camera.main.ScreenToWorldPoint(m_gesList[i].LineEnd));
				break;
			}
		}
	}

	void StartGesture()
	{
		Debug.Log("Start Gesture");
		m_isDrawing = true;
		m_gesList = new List<GesturePart>();
		
		StartGesturePart();
	}

	void StartGesturePart ()
	{
		m_drawTime = 0.0f;		
		m_rawPointList = new List<gPoint>();
		
		m_possLine = true;
		m_possCircle = true;
		m_possSpiral = true;
		
		MarkPoint();
	}

	void StopGesture()
	{
		MarkPoint();
		
		m_isDrawing = false;
	}
	
	void MarkPoint ()
	{
		m_rawPointList.Add(new gPoint(Input.mousePosition + new Vector3(0,0,10), m_drawTime));
	}
}
