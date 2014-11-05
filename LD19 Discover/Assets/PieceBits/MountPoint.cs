using UnityEngine;

public class MountPoint
{
	Transform   m_Mount;
	MountPoint  m_Other;
	PieceScript m_Owner;
	MountPoint  m_Solution;
	int 		m_TypeIndex;
	
	public PieceScript Owner { get { return m_Owner; } }
	public Transform Mount { get { return m_Mount; } }
	public bool Used { get { return (m_Other != null); } }
	public int TypeID { get { return m_TypeIndex; } }
	
	public Color TypeColour 
	{ 
		get 
		{
			switch(m_TypeIndex)
			{
				case 0:  return Color.red;
				case 1:  return Color.green;
				case 2:  return Color.blue;
				case 3:  return Color.magenta;
				case 4:  return Color.yellow;
				case 5:  return Color.cyan;
			}
			
			return Color.white;
		}
	}
	
	public MountPoint Solution 
	{ 
		get { return m_Solution; } 
		set 
		{ 
			m_Solution = value;
			if(m_Solution.m_TypeIndex >= 0)
			{
				m_TypeIndex = m_Solution.m_TypeIndex;
			}
			else
			{
				m_TypeIndex = Owner.GetNewTypeIndex();
				// Don't set the other it will be set
			}
		} 
	}
	
	private MountPoint() {}
	public MountPoint(PieceScript owner, Transform tMount)
	{
		m_TypeIndex = -1;
		m_Solution = null;
		m_Other = null;
		m_Mount = tMount;
		m_Owner = owner;
	}
	
	public bool connectMount(MountPoint other)
	{
		if(other.Used == false)
		{			
			other.m_Other = this;
			m_Other = other;			
			
			// Notify Pieces of Connection
			m_Owner.ConnectBody(m_Other.m_Owner);
				
			return true;
		}
		
		return false;
	}
	
	public bool freeMount()
	{
		if(m_Other != null) 
		{ 
			// TODO :: Notify Pieces of Disconnection
			
			m_Other.m_Other = null; 
			m_Other = null;
			return true;
		}
		
		return false;
	}
}

