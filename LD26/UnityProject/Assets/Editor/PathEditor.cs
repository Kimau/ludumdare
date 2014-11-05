using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor (typeof(PatrolPath))]
public class PathEditor : Editor {

    public override void OnInspectorGUI() 
	{		
		PatrolPath path = target as PatrolPath;
		if((path.pathNodes == null) || (path.pathNodes.Length <= 0))
			path.pathNodes = new Vector3[2];
		
		List<Vector3> ptList = new List<Vector3>(path.pathNodes);
		List<Vector3> removeList = new List<Vector3>();
			
		if(GUILayout.Button("Reset Rotation & Scale"))
		{
			for(int i = 0; i < ptList.Count; ++i)
				ptList[i] = path.transform.TransformPoint(ptList[i]);
			
			path.transform.localScale = Vector3.one;
			path.transform.localRotation = Quaternion.identity;
			
			for(int i = 0; i < ptList.Count; ++i)
				ptList[i] = path.transform.InverseTransformPoint(ptList[i]);
		}
		
		for(int i = 0; i < ptList.Count; ++i)
		{
			EditorGUILayout.BeginHorizontal();
			ptList[i] = EditorGUILayout.Vector3Field(""+1, ptList[i]);
			if(GUILayout.Button("-", GUILayout.Width(20)))
				removeList.Add(ptList[i]);
			EditorGUILayout.EndHorizontal();
		}
		
		if(GUILayout.Button("Add Point"))
		{
			Vector3 newPt = ptList[ptList.Count-1] + Vector3.one;
			ptList.Add(newPt);
		}
		
		while(removeList.Count > 0)
		{
			ptList.Remove(removeList[0]);
			removeList.RemoveAt(0);
		}
		
        if (GUI.changed)
        {
			path.pathNodes = ptList.ToArray();
			EditorUtility.SetDirty (target);
		}
    }
	
    public void OnSceneGUI() 
	{		
		PatrolPath path = target as PatrolPath;	
		if((path.pathNodes == null) || (path.pathNodes.Length <= 0))
			return;
		
		Vector3[] worldPts = new Vector3[path.pathNodes.Length];
		
		for (int i = 0; i < path.pathNodes.Length; i++) 
		{
			worldPts[i] = path.transform.TransformPoint(path.pathNodes[i]);
			worldPts[i] = Handles.FreeMoveHandle(worldPts[i], Quaternion.identity,
				1.0f, Vector3.one, Handles.DotCap);
			
			path.pathNodes[i] = path.transform.InverseTransformPoint(worldPts[i]);
		}
		
		Handles.DrawAAPolyLine(10.0f, worldPts);
		
        if (GUI.changed)
            EditorUtility.SetDirty (target);
    }
}
