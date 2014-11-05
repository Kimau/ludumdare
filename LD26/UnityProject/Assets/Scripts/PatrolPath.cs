using UnityEngine;
using System.Collections;

public class PatrolPath : MonoBehaviour 
{
	public Vector3[] pathNodes;
	
	public int PathLength { get { return pathNodes.Length; } }
	public Vector3 this[int key] { get { return transform.TransformPoint(pathNodes[key]); } }
}
