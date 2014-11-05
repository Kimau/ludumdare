using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PieceScript : MonoBehaviour 
{
	public const int m_MaxMounts = 6;
	
	public bool m_intersecting = false;
	public bool m_inPlay = false;
	
	bool m_hasChildren = false;
	bool m_isChild = false;
	bool m_selected = false;
	TransformLerp m_movingTrans;
	List<MountPoint> m_Mounts;

	// Use this for initialization
	void Awake() 
	{
		m_hasChildren = false;
		m_isChild = false;
		m_selected = false;
		
		// Build a list of mount transforms
		m_Mounts = new List<MountPoint>(transform.GetChildCount());
		
		foreach(Transform iTrans in transform.GetComponentInChildren<Transform>())
		{
			m_Mounts.Add(new MountPoint(this, iTrans));
		}
	}
	
	// Update is called once per frame
	void Update() 
	{
		if(m_movingTrans != null)
		{
			m_movingTrans.Update(Time.deltaTime, transform);
			
			if(m_movingTrans.Active() == false)
			{
				m_movingTrans = null;
			}
		}
	}
	
	public void SaveMeshPosition()
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		
		Vector3[] vertices = mesh.vertices;
        Color[] colors = new Color[vertices.Length];
		
        int i = 0;
        
		while (i < vertices.Length) 
		{
			Vector3 worldPos = transform.TransformPoint(vertices[i]);
            colors[i] = 
				new Color(
					Mathf.Sin(worldPos.x), 
					Mathf.Cos(worldPos.y), 
					Mathf.Sin(worldPos.z), 1.0f);
            ++i;
        }
		
        mesh.colors = colors;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.GetComponent<PieceScript>() != null)
		{
			if(m_inPlay == false)
			{
				StopTesting(true);
			}
			else
			{
				other.GetComponent<PieceScript>().StopTesting(true);
			}
		}
	}
	
	public void StartTesting()
	{
		m_intersecting = false;
		collider.isTrigger = true;
	}
	
	public void StopTesting(bool intersect)
	{
		m_intersecting = intersect;
		collider.isTrigger = false;		
	}
	
	public void SelectOn()
	{
		if(m_selected == false)
		{
			rigidbody.isKinematic = true;
			
			m_movingTrans = new TransformLerp( 1.0f,
			 	new Vector3[]{transform.localPosition, Vector3.zero},
				new Vector3[]{transform.localScale, transform.localScale},
				new Quaternion[]{transform.localRotation, Quaternion.identity} );
		}
		
		m_selected = true;
	}
	
	public void SelectOff()
	{
		if(m_selected == true)
		{
			m_movingTrans = null;
			m_selected = false;
			rigidbody.isKinematic = false;
			rigidbody.AddForce(Vector3.zero);
		}
		
		SpringJoint sJoint = gameObject.GetComponent<SpringJoint>();
		if(sJoint != null)
		{
			Destroy(sJoint);
		}
	}
	
		
	public MountPoint GetNextMount(MountPoint oldMount, bool isUsed, bool careAboutSolution)
	{
		int startPoint = 0;
		if( (oldMount != null) && (m_Mounts.Contains(oldMount)) )
		{
			startPoint = m_Mounts.IndexOf(oldMount);
		}
		else
		{
			if( (m_Mounts[0].Used == isUsed) &&
				((careAboutSolution == false) || (m_Mounts[0].TypeID >= 0)) )
			{
				return m_Mounts[0];
			} 	
		}
		
		// Find Next Point which matches
		int iPoint = (startPoint + 1) % m_Mounts.Count;
		while(iPoint != startPoint)
		{
			if( (m_Mounts[iPoint].Used == isUsed) &&
				((careAboutSolution == false) || (m_Mounts[iPoint].TypeID >= 0)) )
			{
				return m_Mounts[iPoint];
			}
			
			iPoint = (iPoint + 1) % m_Mounts.Count;
		}
		
		return null;
	}
	
	private static bool HasSolution(MountPoint m) { return (m.Solution != null); }	
	private static bool IsFree(MountPoint m) { return (m.Used == false); }
	private static bool IsUsed(MountPoint m) { return (m.Used == true); }
	
	public List<MountPoint> GetFreeMounts()	{ return m_Mounts.FindAll(IsFree); }
	
	public bool AnyEmptyMounts()
	{
		if(m_Mounts.TrueForAll(IsUsed))
		{
			return false;
		}
		
		return true;
	}
	
	public void PlaceInPlay()
	{
		StopTesting(false);
		
		renderer.enabled = true;
		m_inPlay = true;
		SaveMeshPosition();
	}
	
	public void DebugDrawSolution()
	{
		List<MountPoint> usedPoints = m_Mounts.FindAll(HasSolution);
		
		foreach(MountPoint uPoint in usedPoints)
		{
			Debug.DrawLine(uPoint.Mount.position, uPoint.Solution.Mount.position);
		}
	}
	
	public void ConnectBody(PieceScript other)
	{
		if(m_isChild == true)
		{
			transform.parent.GetComponent<PieceScript>().ConnectBody(other);
			return;
		}
		
		while(other.m_isChild)
		{
			other = other.transform.parent.GetComponent<PieceScript>();
		}
		
		if(other.m_hasChildren)
		{
			PieceScript[] subPieces = other.GetComponentsInChildren<PieceScript>();
			foreach(PieceScript sP in subPieces)
			{
				sP.transform.parent = transform;
			}
		}
		
		SelectOff();
		other.SelectOff();
		
		// Child
		other.transform.parent = transform;
		other.m_isChild = true;
		
		// Merge Rigid Body
		rigidbody.centerOfMass = 
			(rigidbody.centerOfMass * rigidbody.mass) +
			(other.rigidbody.centerOfMass * other.rigidbody.mass);
		
		rigidbody.mass += other.rigidbody.mass;
		
		Destroy(other.rigidbody);
		
		// Add Mount Points
		m_Mounts.AddRange(other.GetFreeMounts());
		
		SendMessageUpwards("SelectObj", gameObject);
		m_hasChildren = true;
	}
	
	public int GetNewTypeIndex()
	{		
		// Build Possible List
		// HACK :: Bad Claire this code is not good
		List<int> availList = new List<int>(m_MaxMounts);
		for(int iT = 0; iT < m_MaxMounts; ++iT)
		{
			availList.Add(iT);
		}
		
		foreach(MountPoint iMount in m_Mounts)
		{
			if( (iMount.Owner == this) && (availList.Contains(iMount.TypeID)) )
			{
				availList.Remove(iMount.TypeID);
			}
		}
		
		// TODO :: Replace this with Random Equation
		return (availList.Count > 0)?availList[0]:(-1);
	}
	
}
