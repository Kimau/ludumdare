using UnityEngine;
using System.Collections;

public class KaPuzzle : MonoBehaviour
{
  public GameObject m_Solid;
  public GameObject m_Input;
  public GameObject m_Down;
  public GameObject m_Right;
  public GameObject m_Both;

  public PuzzleData m_puzzle;

  KaBlockInput[] m_inputBlocks;
  GameObject[] m_grid;
  Bounds m_Bounds;

  public int GetIndex(int x, int y)
  {
    return (x + 1) + (m_puzzle.m_width + 1) * (y + 1);
  }

  // Use this for initialization
  void Start()
  {
    if (m_puzzle == null)
      m_puzzle = ScriptableObject.CreateInstance<PuzzleData>();

    m_Bounds = new Bounds(transform.position, Vector3.zero);

    m_grid = new GameObject[(m_puzzle.m_width + 1) * (m_puzzle.m_height + 1)];

    // Build Grid
    for (int x = 0; x < m_puzzle.m_width; x++)
    {
      for (int y = 0; y < m_puzzle.m_height; y++)
      {
        int datVal = m_puzzle.m_data [x + m_puzzle.m_width * y];

        if (datVal > 0)
          CreateBlock(x, y, datVal);
        else
        {
          CreateDirBlock(x, y);
        }
      }
    }

    // Edges
    CreateBlock(-1, -1, 0);
    for (int y = 0; y < m_puzzle.m_height; y++)
      CreateDirBlock(-1, y);
    
    for (int x = 0; x < m_puzzle.m_width; x++)
      CreateDirBlock(x, -1);

    // Frame Camera
    UtilityScripts.FrameBounds(Camera.main, m_Bounds, Vector3.forward);

    //
    m_inputBlocks = GetComponentsInChildren<KaBlockInput>();
  } 

  void CreateBlock(int x, int y, int datVal)
  {
    // Create Object
    GameObject newBlock;
    if ((datVal < 1) || (datVal > 9))
    {
      newBlock = Instantiate(m_Solid) as GameObject;
    } else
    {
      newBlock = Instantiate(m_Input) as GameObject;
      KaBlockInput kbi = newBlock.GetComponent<KaBlockInput>();
      kbi.m_Col = x;
      kbi.m_Row = y;
    }
    // Place Block
    newBlock.transform.parent = transform;
    newBlock.transform.localPosition = new Vector3(x, m_puzzle.m_height - y, 0);
    newBlock.transform.localScale = Vector3.one;
    newBlock.transform.localRotation = Quaternion.identity;
    m_grid [GetIndex(x, y)] = newBlock;

    // Extend Bounds
    m_Bounds.Encapsulate(new Bounds(newBlock.transform.position, Vector3.one));
  }

  void CreateDirBlock(int x, int y)
  {
    int dirRight = 0;
    int dirDown = 0;

    // Count Blocks
    if (y >= 0)
    {
      int modX = x + 1;
      while ((modX < m_puzzle.m_width) && 
             (m_puzzle.m_data[modX + m_puzzle.m_width * y] > 0))
      {
        dirRight += m_puzzle.m_data [modX + m_puzzle.m_width * y];
        modX += 1;
      }
    }

    if (x >= 0)
    {
      int modY = y + 1;
      while ((modY < m_puzzle.m_height) && 
             (m_puzzle.m_data[x + m_puzzle.m_width * modY] > 0))
      {
        dirDown += m_puzzle.m_data [x + m_puzzle.m_width * modY];
        modY += 1;
      }
    }

    // Create Object
    GameObject newBlock;
    if (dirDown > 0)
    {
      if (dirRight > 0)
        newBlock = Instantiate(m_Both) as GameObject;
      else
        newBlock = Instantiate(m_Down) as GameObject;
    } else if (dirRight > 0)
    {
      newBlock = Instantiate(m_Right) as GameObject;
    } else
    {
      newBlock = Instantiate(m_Solid) as GameObject;
    }

    if (dirDown > 0)
      newBlock.SendMessage("SetDown", dirDown);

    if (dirRight > 0)
      newBlock.SendMessage("SetRight", dirRight);

    // Place Block
    newBlock.transform.parent = transform;
    newBlock.transform.localPosition = new Vector3(x, m_puzzle.m_height - y, 0);
    newBlock.transform.localScale = Vector3.one;
    newBlock.transform.localRotation = Quaternion.identity;
    m_grid [GetIndex(x, y)] = newBlock;
    // Extend Bounds
    m_Bounds.Encapsulate(new Bounds(newBlock.transform.position, Vector3.one));
  }
	
  // Update is called once per frame
  void Update()
  {
    Ray mr = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hitInfo;

    int sel = -1;
    for (int i = 0; i < m_inputBlocks.Length; i++)
    {
      if (m_inputBlocks [i].collider.Raycast(mr, out hitInfo, 1000.0f))
      {
        sel = i;
        m_inputBlocks [i].MouseIn();
      } else
        m_inputBlocks [i].MouseOut();
    }

    if (sel >= 0)
    {
      if (Input.GetKeyUp(KeyCode.Backspace))
        SetInput(sel, -1);
      else if (Input.GetKeyUp(KeyCode.Alpha1))
        SetInput(sel, 1);
      else if (Input.GetKeyUp(KeyCode.Alpha2))
        SetInput(sel, 2);
      else if (Input.GetKeyUp(KeyCode.Alpha3))
        SetInput(sel, 3);
      else if (Input.GetKeyUp(KeyCode.Alpha4))
        SetInput(sel, 4);
      else if (Input.GetKeyUp(KeyCode.Alpha5))
        SetInput(sel, 5);
      else if (Input.GetKeyUp(KeyCode.Alpha6))
        SetInput(sel, 6);
      else if (Input.GetKeyUp(KeyCode.Alpha7))
        SetInput(sel, 7);
      else if (Input.GetKeyUp(KeyCode.Alpha8))
        SetInput(sel, 8);
      else if (Input.GetKeyUp(KeyCode.Alpha9))
        SetInput(sel, 9);
    }
  }

  void SetInput(int sel, int val)
  {
    m_inputBlocks [sel].SendMessage("SetVal", val);
  }

  void OnDrawGizmosSelected()
  {
  }
}
