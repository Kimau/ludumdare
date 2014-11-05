using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WordPuzzleGener : MonoBehaviour 
{
	public GameObject m_PlayArea;
	public GameObject[] m_BaseBlocks;
	public string m_PassPhrase;
	public PuzzleCamera m_puzCam;
	public Collider m_MousePin;
	
	public ParticleEmitter m_PartSlot;
	public ParticleEmitter m_PartTab;
	
	Bounds m_gameArea;
	
	// Used only for Generation
	bool m_isScatterReady = false;
	bool m_isTestComplete = false;
	
	int m_iLetter = 0;
	int m_testIncr = 0;
	MountPoint m_TestSlot = null;
	MountPoint m_TestTab = null;
	List<MountPoint> m_TestSlotList = null;
	List<MountPoint> m_TestTabList = null;
	
	List<MountPoint> m_FreePoints;		
	GameObject[] m_MagicBlocks;
	
	// Used for Game
	Rigidbody m_DragBody;
	PieceScript m_SelectPiece;
	GameObject[] m_PuzzleBlocks;
	bool m_isDragging = false;
	bool m_isValidConnection = false;
	
	MountPoint m_SelectMount = null;
	MountPoint m_NearestMount = null;
	
	#region Generate Puzzle
	
	// Use this for initialization
	void Start() 
	{
		print("We have " + m_BaseBlocks.Length + " blocks");
		print(Time.fixedDeltaTime);
		Time.fixedDeltaTime = 0.0001f;
		
		m_PartSlot.enabled = false;
		m_PartTab.enabled = false;
		
		Renderer[] groupRenderer = m_PlayArea.GetComponentsInChildren<Renderer>();
		foreach( Renderer cr in groupRenderer )
		{
			cr.enabled = false;
		}
		
		// Sanity Check Word Block List
		int numError = 0;
		for( int i = 0; i < m_BaseBlocks.Length; ++i)
		{
			if(m_BaseBlocks[i].GetComponent<PieceScript>() == null)
			{
				print("Error using Piece [" + m_BaseBlocks[i].name + "]");
				numError += 1;
			}
		}
		
		// BAD BAD BAD Claire
		int newLength = m_BaseBlocks.Length - numError;
		int safeIter = 0;
		m_MagicBlocks = new GameObject[newLength];
		for( int iBlock = 0; iBlock < newLength; ++iBlock)
		{
			if(m_BaseBlocks[iBlock].GetComponent<PieceScript>() != null)
			{
				m_MagicBlocks[safeIter] = (GameObject)Instantiate(m_BaseBlocks[iBlock], Vector3.zero, Quaternion.identity);
				m_MagicBlocks[safeIter].active = false;
				
				safeIter += 1;
			}			
		}
		
		print("We have " + m_MagicBlocks.Length + " magical blocks");

		m_FreePoints = new List<MountPoint>();
		m_PuzzleBlocks = new GameObject[m_PassPhrase.Length];
		
		// Pick Block
		int blockID = (m_PassPhrase[0]) % m_MagicBlocks.Length;
		GameObject currBlock = (GameObject)Instantiate(m_MagicBlocks[blockID]);
		
		// Root Node
		currBlock.transform.localPosition = Vector3.zero;
		currBlock.transform.rotation = Quaternion.identity;
		currBlock.transform.localScale = Vector3.one;
		currBlock.name = "Letter 0";
		currBlock.transform.parent = transform;
		
		// Mark as in Play
		PieceScript currBlockData = currBlock.GetComponent<PieceScript>();
		currBlockData.PlaceInPlay();
		
		// Add Mount Points
		m_FreePoints.AddRange(currBlockData.GetFreeMounts());
		
		m_PuzzleBlocks[0] = currBlock;
		
		// Generate all other blocks
		m_iLetter = 1;
		GenerateBlockLetter(true);
	}
	
	void ScatterPieces()
	{
		if(Input.GetKeyUp(KeyCode.Space))
		{
			m_isScatterReady = false;
		}
		else
		{
			return;
		}
		
		// Scatter Time
		m_gameArea = new Bounds(Vector3.zero, Vector3.one);
		
		foreach( GameObject block in m_PuzzleBlocks )
		{
			m_gameArea.Encapsulate(block.collider.bounds);
			block.rigidbody.isKinematic = false;
			block.rigidbody.useGravity = true;
			block.rigidbody.AddExplosionForce(2000.0f, Vector3.zero, 10000.0f);
		}
		
		m_gameArea.Expand(10.0f);
		m_PlayArea.transform.localPosition = m_gameArea.center;
		m_PlayArea.transform.localScale = m_gameArea.size;
		
		m_MousePin.transform.position = m_PlayArea.transform.localPosition + (Vector3.down * m_PlayArea.transform.localScale.y * 0.5f);
		m_puzCam.SelectObj(m_MousePin.gameObject);
	}
	
	public IEnumerator AwaitingTest() 
	{
		m_isTestComplete = false;
		yield return new WaitForFixedUpdate();

		m_isTestComplete = true;
		
		TestBlock();
	}
	

	void TestBlock()
	{
		// print("Testing Letter " + m_iLetter);
		PieceScript currBlockData = m_PuzzleBlocks[m_iLetter].GetComponent<PieceScript>();
		
		// Did we fail
		if(currBlockData.m_intersecting == true)
		{
			// print("Trying new Position");
			m_testIncr += 1;
			
			GenerateBlockLetter(false);
			return;
		}
		
		// Fitted in
		currBlockData.PlaceInPlay();
		m_TestSlot.Solution = m_TestTab;
		m_TestTab.Solution = m_TestSlot;
		
		// Add Free Points then Remove Points we Used
		m_FreePoints.AddRange(m_TestTab.Owner.GetFreeMounts());
		m_FreePoints.Remove(m_TestSlot);
		m_FreePoints.Remove(m_TestTab);
		
		// Move onto Next Letter
		m_testIncr = 0;
		++m_iLetter;
		if(m_iLetter >= m_PassPhrase.Length)
		{		
			// Cleanup
			for( int i = 0; i < m_MagicBlocks.Length; ++i)
			{
				Destroy(m_MagicBlocks[i]);
			}
			
			Time.fixedDeltaTime = 0.02f;
			m_MagicBlocks = null;
			print("Done Generating");
			m_isScatterReady = true;
			return;
		}
		
		m_PuzzleBlocks[m_iLetter] = null;
		GenerateBlockLetter(true);
	}
	
	void GenerateBlockLetter(bool forceNewLetter)
	{
		// print("Generating Letter " + m_iLetter);
		
		// TODO :: Improve this number
		int randNum = m_iLetter + m_PassPhrase[m_iLetter] + m_testIncr;
		
		// Do we need to refresh Slot List and Pick new Tab
		if( (forceNewLetter == true) || 
			(m_TestSlotList == null) || 
			(m_TestSlotList.Count < 1) )
		{
			// Do we need to pick a new block
			if( (forceNewLetter == true) || 
				(m_TestTabList == null) || 
				(m_TestTabList.Count < 1) )
			{
				if(m_PuzzleBlocks[m_iLetter] != null)
				{
					Destroy(m_PuzzleBlocks[m_iLetter]);
				}
				
				// Pick Block
				int blockID = randNum % m_MagicBlocks.Length;
				// TODO :: Increment should affect this choice but doesn't
				
				GameObject currBlock = (GameObject)Instantiate(m_MagicBlocks[blockID]);
				currBlock.transform.parent = transform;
				m_PuzzleBlocks[m_iLetter] = currBlock;
				m_PuzzleBlocks[m_iLetter].active = true;
				m_PuzzleBlocks[m_iLetter].renderer.enabled = false;
				
				// Get List
				m_TestTabList = m_PuzzleBlocks[m_iLetter].GetComponent<PieceScript>().GetFreeMounts();
			}
		
			// Get New Test Tab
			m_TestTab = m_TestTabList[(randNum + m_TestTabList.Count) % m_TestTabList.Count];
			m_TestTabList.Remove(m_TestTab);
			
			// Refresh New Slot List
			m_TestSlotList = new List<MountPoint>(m_FreePoints);	// Copy List
		}
		
		// Get New Slot
		m_TestSlot = m_TestSlotList[(randNum + m_TestSlotList.Count) % m_TestSlotList.Count];
		
		// Recentre Piece
		m_PuzzleBlocks[m_iLetter].transform.localPosition = Vector3.zero;
		m_PuzzleBlocks[m_iLetter].transform.rotation = Quaternion.identity;
		m_PuzzleBlocks[m_iLetter].transform.localScale = Vector3.one;		

		// Rotate Piece
		Quaternion newRot = Quaternion.identity;
		newRot.SetFromToRotation(
			m_TestTab.Mount.up * -1.0f,
			m_TestSlot.Mount.up);
			
		m_PuzzleBlocks[m_iLetter].transform.localRotation *= newRot;
		
		// Position Piece
		Vector3 diff = m_TestSlot.Mount.position - m_TestTab.Mount.position;
		m_PuzzleBlocks[m_iLetter].transform.Translate(diff, Space.World);
		
		// Check it fits
		m_PuzzleBlocks[m_iLetter].GetComponent<PieceScript>().StartTesting();
		m_PuzzleBlocks[m_iLetter].name = "Letter " + m_iLetter;
		
		StartCoroutine(AwaitingTest());
	}
	
	#endregion
	
	// Update is called once per frame
	void Update() 
	{
		if(m_isTestComplete == false)
		{
			if(m_TestTab != null)  { DrawTrans(m_TestTab.Mount); }
			if(m_TestSlot != null) { DrawTrans(m_TestSlot.Mount); }
			
			return;
		}		
	
		/* HACK :: Show Joins
		foreach(GameObject pB in m_PuzzleBlocks)
		{			
			PieceScript magicSel = pB.GetComponent<PieceScript>();
			magicSel.DebugDrawSolution();
		}
		/**/
		
		if(m_isScatterReady)
		{			
			ScatterPieces();
			return; // This is to stop pre-mature interaction (dirty I know)
		}
		
		// TODO :: Need to look into the options of a click vs drag
		if(Input.GetMouseButtonUp(0))
		{
			RaycastHit hitInfo;
			
			if(Physics.Raycast(m_puzCam.camera.ScreenPointToRay(Input.mousePosition), out hitInfo))
			{
				if(hitInfo.rigidbody != m_DragBody)
				{
					SelectObj(hitInfo.rigidbody.gameObject);
				}
			}
		}
		
		UpdateDragInput();
		
		// Update Particle Systems
		if(m_SelectMount != null)
		{
			Color slotColour = m_SelectMount.TypeColour;
			
			Color[] newColours = new Color[5];
			newColours[0] = new Color(slotColour.r, slotColour.g, slotColour.b, 0.3f);
			newColours[1] = new Color(slotColour.r, slotColour.g, slotColour.b, 0.6f);
			newColours[2] = new Color(slotColour.r, slotColour.g, slotColour.b, 1.0f);
			newColours[3] = new Color(slotColour.r, slotColour.g, slotColour.b, 0.8f);
			newColours[4] = new Color(slotColour.r, slotColour.g, slotColour.b, 0.1f);
			
			m_PartTab.GetComponent<ParticleAnimator>().colorAnimation = newColours;
			m_PartTab.transform.position = m_SelectMount.Mount.position;
			m_PartTab.transform.rotation = m_SelectMount.Mount.rotation;
			m_PartTab.enabled = true;
		}
		else
		{
			m_PartTab.enabled = false;
			m_PartTab.ClearParticles();
		}
		
		if(m_NearestMount != null)
		{			
			Color slotColour = m_NearestMount.TypeColour;			
			
			Color[] newColours = new Color[5];
			newColours[0] = new Color(slotColour.r, slotColour.g, slotColour.b, 0.3f);
			newColours[1] = new Color(slotColour.r, slotColour.g, slotColour.b, 0.6f);
			newColours[2] = new Color(slotColour.r, slotColour.g, slotColour.b, 1.0f);
			newColours[3] = new Color(slotColour.r, slotColour.g, slotColour.b, 0.8f);
			newColours[4] = new Color(slotColour.r, slotColour.g, slotColour.b, 0.1f);
			
			m_PartSlot.GetComponent<ParticleAnimator>().colorAnimation = newColours;
			m_PartSlot.transform.position = m_NearestMount.Mount.position;
			m_PartSlot.transform.rotation = m_NearestMount.Mount.rotation;
			m_PartSlot.enabled = true;
		}
		else
		{
			m_PartSlot.enabled = false;
			m_PartTab.ClearParticles();
		}
	}
	
	void UpdateDragInput()
	{
		RaycastHit hitInfo;
		
		if(Input.GetMouseButtonDown(1) && (m_SelectPiece != null))
		{			
			if(Physics.Raycast(m_puzCam.camera.ScreenPointToRay(Input.mousePosition), out hitInfo))
			{
				if(hitInfo.rigidbody != m_SelectPiece.rigidbody)
				{
					m_DragBody = hitInfo.rigidbody;
					m_SelectMount = m_DragBody.GetComponent<PieceScript>().GetNextMount(null, false, true);
					print(m_SelectMount);
					
					m_isDragging = true;
					m_isValidConnection = false;
					
					m_DragBody.isKinematic = true;
				}
			}
		}
		
		if(m_isDragging)
		{	
			// Rotate and Move point
			PieceScript floater = m_DragBody.GetComponent<PieceScript>();
			
			if(Input.GetKeyUp(KeyCode.Space))
			{
				m_SelectMount = m_DragBody.GetComponent<PieceScript>().GetNextMount(m_SelectMount, false, true);
				print(m_SelectMount);
			}
			
			if((m_SelectPiece != null) && (m_SelectMount != null))
			{			
				// Position Hit Bubble
				m_MousePin.transform.position = m_SelectPiece.transform.position;
				
				// Check if we hit bubble
				Ray mouseRay = m_puzCam.camera.ScreenPointToRay(Input.mousePosition);
				RaycastHit sphereHitInfo;		
				
				
				if(m_MousePin.Raycast(mouseRay, out sphereHitInfo, 1000.0f))
				{
					MoveToClosestAnchor(sphereHitInfo, floater);
				}
				else
				{
					Vector3 newPos;
					
					if(sphereHitInfo.distance < 0.00001f)
					{						
						// Setup Infinite Plane
						Plane hitPlane = new Plane(m_puzCam.transform.forward, m_SelectPiece.transform.position);
						float rayDist = 1.0f;
						hitPlane.Raycast(mouseRay, out rayDist);
						newPos = mouseRay.GetPoint(rayDist);
					}
					else
					{
						newPos = sphereHitInfo.point;
					}
					
					floater.transform.position = 
						Vector3.Lerp(floater.transform.position, newPos, 0.15f);
				}
			
			}
			
			if(Input.GetMouseButtonUp(1))
			{
				m_isDragging = false;
				m_DragBody.isKinematic = false;
				
				WeldPiece(floater);
				
				m_DragBody = null;
				m_SelectMount = null;
			}
		}
		
	}

	void MoveToClosestAnchor(RaycastHit sphereHitInfo, PieceScript floater)
	{
		float mountDist = 10000000000.0f;
		
		m_NearestMount = null;		
		m_isValidConnection = false;
		
		// Locate Nearest Mount Point
		foreach(MountPoint iMount in m_SelectPiece.GetFreeMounts())
		{
			float mDist = (iMount.Mount.position - sphereHitInfo.point).magnitude;
			
			if((iMount.TypeID >= 0) && (mDist < mountDist))
			{
				mountDist = mDist;
				m_NearestMount = iMount;
				m_isValidConnection = true;
			}
		}
		
		// Align Floating Piece
		Quaternion qAlter = Quaternion.FromToRotation(
			m_SelectMount.Mount.localRotation * Vector3.down,
			m_NearestMount.Mount.up);
		
		if(m_SelectMount.Owner != floater)
		{
			// There MUST be a cleaner way todo this			
			Vector3 newDown =
				m_SelectMount.Owner.transform.localRotation * 
				m_SelectMount.Mount.localRotation * 
				Vector3.down;
			
			qAlter = Quaternion.FromToRotation(
				newDown,
				m_NearestMount.Mount.up);
		}
		
	
		floater.transform.rotation = Quaternion.Lerp(floater.transform.rotation, qAlter, 0.1f);	
		
		// Move Point
		Vector3 tarPoint = 
			m_NearestMount.Mount.position - 
			(
					m_SelectMount.Mount.position -
					floater.transform.position
			);
		
		if(m_isValidConnection == false)
		{
			tarPoint = tarPoint + m_NearestMount.Mount.up;
		}
		
		// Move to Mount
		floater.transform.position =
			Vector3.Lerp(floater.transform.position, tarPoint, 0.05f);
		
		// Draw Line to indicate Bubble
		Debug.DrawLine(
			m_SelectMount.Mount.position,
			m_NearestMount.Mount.position);
		
		DrawTrans(m_SelectMount.Mount.transform);
		DrawTrans(m_NearestMount.Mount.transform);
	}

	void WeldPiece(PieceScript floater)
	{
		if((m_SelectMount == null) || (m_NearestMount == null))
		{
			return;
		}
		
		print("Weld Piece " + m_SelectMount.Owner.name + " to " + m_NearestMount.Owner.name);
			
		// Ensure Rotation & Position
		floater.transform.rotation = Quaternion.FromToRotation(
			m_SelectMount.Mount.localRotation * Vector3.down,
			m_NearestMount.Mount.up);
		
		floater.transform.position =						
			m_NearestMount.Mount.position - 
				(m_SelectMount.Mount.position -	floater.transform.position);
		
		// Check if its a valid piece
		if(m_SelectMount.Solution == m_NearestMount)
		{
			// Connect
			m_SelectMount.connectMount(m_NearestMount);
		}
		else
		{			
			// Bounce Away
			floater.rigidbody.AddForce(m_NearestMount.Mount.up * 1000.0f);
			floater.rigidbody.AddForceAtPosition(m_SelectMount.Mount.right * 50.0f, m_SelectMount.Mount.position);
		}
	}
	
	static void DrawTrans(Transform drawTrans)
	{
		Debug.DrawLine(drawTrans.position, drawTrans.TransformPoint(Vector3.up), Color.red);
		Debug.DrawLine(drawTrans.position, drawTrans.TransformPoint(Vector3.left), Color.green);
		Debug.DrawLine(drawTrans.position, drawTrans.TransformPoint(Vector3.forward), Color.blue);
	}
	
	
	void SelectObj(GameObject newSelection)
	{
		PieceScript[] puzzleBits = GetComponentsInChildren<PieceScript>();
		m_SelectPiece = null;
		
		foreach(PieceScript bit in puzzleBits)
		{
			if(newSelection == bit.gameObject)
			{
				bit.SelectOn();
				m_SelectPiece = bit;
				
				m_NearestMount = m_SelectPiece.GetNextMount(null, false, true);
			}
			else
			{
				bit.SelectOff();
			}
		}
		
		m_puzCam.SelectObj(newSelection);
	}

}
