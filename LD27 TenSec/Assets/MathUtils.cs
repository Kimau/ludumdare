using UnityEngine;
using System.Collections;

public class MathUtils
{
	public static Vector2 Rot(Vector2 pt, float ang)
	{
		return new Vector2(	pt.x * Mathf.Cos(ang) - pt.y * Mathf.Sin(ang),
							pt.x * Mathf.Sin(ang) + pt.y * Mathf.Cos(ang));
	}
	
	public static bool IsIntersecting(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, out Vector2 pt)
	{
		pt = A1;
		
		float den  = ((A2.x - A1.x) * (B2.y - B1.y)) - ((A2.y - A1.y) * (B2.x - B1.x));
		float num1 = ((A1.y - B1.y) * (B2.x - B1.x)) - ((A1.x - B1.x) * (B2.y - B1.y));
		float num2 = ((A1.y - B1.y) * (A2.x - A1.x)) - ((A1.x - B1.x) * (A2.y - A1.y));

		// Detect coincident lines (has a problem, read below)
		if(den == 0)
			return (num1 == num2);

		num1 = num1 / den;
		num2 = num2 / den;
	
		bool result = (num1 >= 0 && num1 <= 1) && (num2 >= 0 && num2 <= 1);
		pt = A1 + (num1 * (A2 - A1));
		return result;
	}
	
	public static void CalcCircle(Vector2[] pts, out Vector2 center, out float radius)
	{
		Vector2 AB = pts[1] - pts[0];
		Vector2 AC = pts[2] - pts[0];
		
		Vector2 ABx = new Vector2(-AB.y, AB.x); // 90deg
		Vector2 ACx = new Vector2(-AC.y, AC.x); // 90deg
		
		Vector2 aMid = (pts[1] + pts[0]) * 0.5f;
		Vector2 bMid = (pts[2] + pts[0]) * 0.5f;
		
		ABx = ABx.normalized * 10.0f;
		ACx = ACx.normalized * 10.0f;
		
		Gizmos.color = Color.magenta;
		
		Gizmos.DrawLine(ToWorld(aMid-ABx), ToWorld(aMid+ABx));
		Gizmos.DrawLine(ToWorld(bMid-ACx), ToWorld(bMid+ACx));
				
		IsIntersecting(aMid, aMid + ABx, bMid, bMid + ACx, out center);
		radius = (pts[0] - center).magnitude;
		//center += pts[0];
			
		for(float x = 0.0f; x < (Mathf.PI * 2.0f); x += 0.3f)
			Gizmos.DrawLine(
				ToWorld(center + Rot(new Vector2(radius, 0.0f), x)), 
				ToWorld(center + Rot(new Vector2(radius, 0.0f), x+0.3f)));
	}
}
