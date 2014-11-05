using UnityEngine;
using System.Collections;

public class UtilityScripts
{

  public static void FrameBounds(Camera cam, Bounds toFrameBounds, Vector3 lookDir)
  {
    Vector3 max = toFrameBounds.size;
    float radius = Mathf.Max(max.x, Mathf.Max(max.y, max.z));
    float dist = radius / (Mathf.Sin(cam.fieldOfView * Mathf.Deg2Rad));

    // Debug.Log("Radius = " + radius + " dist = " + dist);

    Vector3 pos = toFrameBounds.center - lookDir * dist;
    cam.transform.position = pos;
    cam.transform.LookAt(toFrameBounds.center);
  }
}
