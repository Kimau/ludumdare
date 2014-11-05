using UnityEngine;
using System.Collections;

public class digCamControl : MonoBehaviour
{
  public Collider keySubjet;

  // Use this for initialization
  void Start()
  {
	
  }
	
  // Update is called once per frame
  void Update()
  {
    Ray mr;
    RaycastHit hitInfo = new RaycastHit();

    Vector3 moveVec = Vector3.zero;

    if (Input.GetKey(KeyCode.D))
    {
      mr = Camera.main.ViewportPointToRay(new Vector3(0.7f, 0.5f, 0.5f));
      if (keySubjet.Raycast(mr, out hitInfo, 10000.0f))
        moveVec += transform.TransformDirection(Vector3.right);
    }

    if (Input.GetKey(KeyCode.A))
    {
      mr = Camera.main.ViewportPointToRay(new Vector3(0.3f, 0.5f, 0.5f));
      if (keySubjet.Raycast(mr, out hitInfo, 10000.0f))
        moveVec -= transform.TransformDirection(Vector3.right);
    }

    if (Input.GetKey(KeyCode.W))
    {
      mr = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.7f, 0.5f));
      if (keySubjet.Raycast(mr, out hitInfo, 10000.0f))
        moveVec += transform.TransformDirection(Vector3.up);
    }
    
    if (Input.GetKey(KeyCode.S))
    {
      mr = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.3f, 0.5f));
      if (keySubjet.Raycast(mr, out hitInfo, 10000.0f))
        moveVec -= transform.TransformDirection(Vector3.up);
    }

    // Remove Zoom
    if (moveVec.sqrMagnitude > 0.001f)
      moveVec -= Vector3.up * Vector3.Dot(moveVec, Vector3.up);

    // Zoom Bit
    if (Input.GetKey(KeyCode.Q))
    {
      mr = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
      if ((keySubjet.Raycast(mr, out hitInfo, 10000.0f)) && (hitInfo.distance > 100.0f))
        moveVec += transform.TransformDirection(Vector3.forward);
    }

    if (Input.GetKey(KeyCode.E))
    {
      mr = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
      if ((keySubjet.Raycast(mr, out hitInfo, 10000.0f)) && (hitInfo.distance < 1700.0f))
        moveVec -= transform.TransformDirection(Vector3.forward);
    }

    if (moveVec.sqrMagnitude > 0.001f)
    {
      moveVec.Normalize();
      transform.position += moveVec * Time.deltaTime * (hitInfo.distance - 100.0f) / 1700.0f * 5000.0f;
    }
	
  }
}
