using UnityEngine;
using System.Collections;

public class TorchMeshMaker : MonoBehaviour 
{	
	public float torchDist = 50.0f;
	public float degSpread = 30.0f;
	public int numCasts = 8;
	
	Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;
	
	Mesh torchMesh;
	MeshFilter mf;
	
    void Start() {
        torchMesh = new Mesh();
        mf = GetComponent<MeshFilter>();
		if(mf == null)
			mf = gameObject.AddComponent<MeshFilter>();
		
		mf.mesh = torchMesh;
        torchMesh.vertices = newVertices;
        torchMesh.uv = newUV;
        torchMesh.triangles = newTriangles;
    }
	
	void Update()
	{
		newVertices = new Vector3[1+numCasts];
		newUV = new Vector2[1+numCasts];
		newTriangles = new int[3*(numCasts-1)];
		
		newVertices[0] = Vector3.zero;
		//newUV[0] = new Vector2(0.5f, 0);
		newUV[0] = new Vector2(0, 0);
		
		
		// Triangles
		int t = 0;
		for(int i = 1; i < numCasts; ++i)
		{
			newTriangles[t++] = 0;
			newTriangles[t++] = i;
			newTriangles[t++] = i+1;
		}
		
		// Cast Cone
		// Improve later to use corner finding for better fitting mesh
		float currRot = degSpread * -0.5f;
		float rotStep = degSpread / (numCasts-1);
		float uvStep = 2.0f / (numCasts-1); //float uvStep = 1.0f / (numCasts-1);
		
		for(int i = 1; i < newVertices.Length; ++i)
		{
			RaycastHit hitInfo;
			Vector3 rayDir = Quaternion.AngleAxis(currRot, transform.up) * transform.forward;			
			Vector3 meshRayDir = Quaternion.AngleAxis(currRot, transform.up) * Vector3.forward;
			
			newVertices[i] = meshRayDir * torchDist;
			
			if(i <= (newVertices.Length / 2))
				newUV[i] = new Vector2((i-1)*uvStep, 1.0f);
			else
				newUV[i] = new Vector2(2.0f - (i-1)*uvStep, 1.0f);
			
			if(Physics.Raycast(transform.position, rayDir, out hitInfo, torchDist))
			{
				if(hitInfo.collider.tag == "Player")
					SendMessageUpwards("FoundPlayer", hitInfo.point);
				
				newVertices[i] = meshRayDir * hitInfo.distance;
				// newUV[i] = new Vector2(newUV[i].x, hitInfo.distance / torchDist);
			}
			
			currRot += rotStep;
		}
		
		// Update Mesh
        torchMesh.Clear();
        torchMesh.vertices = newVertices;
        torchMesh.uv = newUV;
        torchMesh.triangles = newTriangles;
    }
	
	void OnDrawGizmosSelected()
	{
		float currRot = degSpread * -0.5f;
		float rotStep = degSpread / (numCasts-1);
		
		for(int i = 0; i < numCasts; ++i)
		{
			RaycastHit hitInfo;
			Vector3 rayDir = Quaternion.AngleAxis(currRot, transform.up) * transform.forward;
			Gizmos.DrawRay(transform.position, rayDir * torchDist);
			
			if(Physics.Raycast(transform.position, rayDir, out hitInfo, torchDist))
			{
				Gizmos.DrawSphere(hitInfo.point, 2.0f);
			}
			
			currRot += rotStep;
		}
	}
	
}
