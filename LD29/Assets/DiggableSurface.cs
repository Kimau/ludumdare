using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class DiggableSurface : MonoBehaviour
{
  [System.Serializable]
  public class GameStats
  {
    // Digging Stats
    public float m_timeTaken;
    public float m_xRayTime;
    public int m_dirtDug;
    public int m_stoneDug;

    // Level Stats
    public int m_gameMode;
    public int m_levelRes;
    public int m_levelDepth;

    // Sucess
    public int m_dinoDestroyed;
    public int m_dinoExposed;
    public int m_dinoBlocks;    // Same for Each Dino Number
    public int m_dinoSurfArea;  // Same for Each Dino Number

    // Dino Stats
    public int m_dinoNumber;
    public int m_dinoX;
    public int m_dinoY;
    public int m_dinoZ;
    public bool m_dinoFlippedX;
    public bool m_dinoFlippedY;

    public GameStats()
    {
      m_timeTaken = 0.0f;
      m_xRayTime = 0.0f;
      m_dirtDug = 0;
      m_stoneDug = 0;

      // Level Stats
      m_gameMode = 0;
      m_levelRes = 0;
      m_levelDepth = 0;
      
      // Sucess
      m_dinoDestroyed = 0;
      m_dinoExposed = 0;
      m_dinoBlocks = 0;
      m_dinoSurfArea = 0;
      
      // Dino Stats
      m_dinoNumber = 0;
      m_dinoX = 0;
      m_dinoY = 0;
      m_dinoZ = 0;
      m_dinoFlippedX = false;
      m_dinoFlippedY = false;
    }

    public void SavePrefs()
    {
      PlayerPrefs.SetFloat("GameState timeTaken", m_timeTaken);
      PlayerPrefs.SetFloat("GameState xRayTime", m_xRayTime);
      PlayerPrefs.SetInt("GameState dirtDug", m_dirtDug);
      PlayerPrefs.SetInt("GameState stoneDug", m_stoneDug);
      
      // Level Stats
      PlayerPrefs.SetInt("GameState gameMode", m_gameMode);
      PlayerPrefs.SetInt("GameState levelRes", m_levelRes);
      PlayerPrefs.SetInt("GameState levelDepth", m_levelDepth);
      
      // Sucess
      PlayerPrefs.SetInt("GameState dinoDestroyed", m_dinoDestroyed);
      PlayerPrefs.SetInt("GameState dinoExposed", m_dinoExposed);
      PlayerPrefs.SetInt("GameState dinoBlocks", m_dinoBlocks);
      PlayerPrefs.SetInt("GameState dinoSurfArea", m_dinoSurfArea);
      
      // Dino Stats
      PlayerPrefs.SetInt("GameState dinoNumber", m_dinoNumber);
      PlayerPrefs.SetInt("GameState dinoX", m_dinoX);
      PlayerPrefs.SetInt("GameState dinoY", m_dinoY);
      PlayerPrefs.SetInt("GameState dinoZ", m_dinoZ);
      PlayerPrefs.SetInt("GameState dinoFlippedX", m_dinoFlippedX ? 1 : 0);
      PlayerPrefs.SetInt("GameState dinoFlippedY", m_dinoFlippedY ? 1 : 0);

      PlayerPrefs.Save();
    }

    public static GameStats LoadPrefs()
    {
      GameStats gs = new GameStats();
      gs.m_timeTaken = PlayerPrefs.GetFloat("GameState timeTaken");
      gs.m_xRayTime = PlayerPrefs.GetFloat("GameState xRayTime");
      gs.m_dirtDug = PlayerPrefs.GetInt("GameState dirtDug");
      gs.m_stoneDug = PlayerPrefs.GetInt("GameState stoneDug");
      
      // Level Stats
      gs.m_gameMode = PlayerPrefs.GetInt("GameState gameMode");
      gs.m_levelRes = PlayerPrefs.GetInt("GameState levelRes");
      gs.m_levelDepth = PlayerPrefs.GetInt("GameState levelDepth");
      
      // Sucess
      gs.m_dinoDestroyed = PlayerPrefs.GetInt("GameState dinoDestroyed");
      gs.m_dinoExposed = PlayerPrefs.GetInt("GameState dinoExposed");
      gs.m_dinoBlocks = PlayerPrefs.GetInt("GameState dinoBlocks");
      gs.m_dinoSurfArea = PlayerPrefs.GetInt("GameState dinoSurfArea");
      
      // Dino Stats
      gs.m_dinoNumber = PlayerPrefs.GetInt("GameState dinoNumber");
      gs.m_dinoX = PlayerPrefs.GetInt("GameState dinoX");
      gs.m_dinoY = PlayerPrefs.GetInt("GameState dinoY");
      gs.m_dinoZ = PlayerPrefs.GetInt("GameState dinoZ");
      gs.m_dinoFlippedX = PlayerPrefs.GetInt("GameState dinoFlippedX") > 0;
      gs.m_dinoFlippedY = PlayerPrefs.GetInt("GameState dinoFlippedY") > 0;

      return gs;
    }

  }

  ///
  public class BrushTemplate
  {
    public float[] m_strength;
    public int m_halfWidth;
    public int m_halfHeight;

    public void SetBrush(Texture2D brushTemplate)
    {
      m_halfWidth = brushTemplate.width / 2;
      m_halfHeight = brushTemplate.height / 2;
      m_strength = new float[m_halfWidth * m_halfHeight * 4];

      Color32[] colArray = brushTemplate.GetPixels32();
      float mod = 1.0f / (256.0f * 3) * 1.0f / 256.0f;
      for (int i = 0; i < colArray.Length; i++)
        m_strength [i] = (colArray [i].r + colArray [i].g + colArray [i].b) * colArray [i].a * mod;
    }
  };
  /// 

  // Events Signup
  public bool m_IsSetup = false;

  // Key Params
  public float m_timerLimit;
  public int m_TexRes = 512;
  public int m_MaxDepth = 64;

  // GUI
  public GUIText m_textExposed;
  public GUIText m_textDamaged;
  public GUIText m_textTool;

  public string[] m_toolNames;
  public string[] m_toolStrength;
  public Font m_guiFont;
  public Texture2D m_buttonBackTex;
  public Texture2D m_buttonBackTexActive;

  GUIStyle m_buttonStyle;

  // Current Brush
  public GameObject m_brushObj;
  public Texture2D[] m_brushTemplate;
  public int m_brushID;
  public int m_brushStrength;
  float[] m_brushStrVal = {8.0f, 2.0f, 0.6f, 0.1f};

  // Dinosaaaaaws
  public int m_dinoSurf;
  public int m_dinoBlock;
  public int m_dinoDestroyed;
  public int m_dinoExposed;
  public Texture2D[] m_dinoList; 

  // Tracked Stats
  float m_startTime;
  public GameStats m_stats;

  // Dig Data
  byte[] m_damageMap;
  byte[] m_dugDepth;
  byte[] m_digData;
  
  // Internals
  BoxCollider m_boxCollider;
  BrushTemplate[] m_brush;
  bool m_isTexDirty;
  bool m_xRayOn;
  float m_timSinceBrush;
  GoogleTracker m_gTrack;

  // 0 Air
  // 1 Dirt
  // 2 Stone
  // 10 Bone
  // 255 SOLID

  public byte Depth(int i)
  {
    return m_dugDepth [i];
  }

  public byte Data(int i)
  {
    return m_digData [i];
  }
  
  // Use this for initialization
  void Start()
  {
    m_IsSetup = false;

    ///////////////////////////////////////////////////////////////
    // Game Stats
    m_stats = new GameStats();
    m_stats.m_gameMode = PlayerPrefs.GetInt("GameState gameMode");
    m_stats.m_levelRes = m_TexRes;
    m_stats.m_levelDepth = m_MaxDepth;

    ///////////////////////////////////////////////////////////////

    m_timSinceBrush = 0.0f;

    // Get Component
    m_boxCollider = GetComponent<BoxCollider>();

    // Brush
    m_brushID = 0;
    m_brushStrength = 0;
    m_brush = new BrushTemplate[m_brushTemplate.Length];
    for (int i = 0; i < m_brushTemplate.Length; i++)
    {
      m_brush [i] = new BrushTemplate();
      m_brush [i].SetBrush(m_brushTemplate [i]);
    }

    UpdateBrush();

    // Setup Dig Texture
    m_dugDepth = new byte[m_TexRes * m_TexRes];
    for (int i = 0; i < m_dugDepth.Length; i++)
      m_dugDepth [i] = (byte)Mathf.Max(0, m_MaxDepth - Random.Range(1, 3));

    // Create Dig Data and fill with Dirt
    m_digData = new byte[m_TexRes * m_TexRes * m_MaxDepth];
    for (int i = 0; i < m_digData.Length; i++)
      m_digData [i] = 1;

    ///////////////////////////////////////////////////////////////
    // Bedrock
    for (int x = 0; x < m_TexRes; x++)
      for (int y = 0; y < m_TexRes; y++)
        m_digData [x + y * m_TexRes + 0] = 255;

    for (int x = 0; x < m_TexRes; x++)
    {
      for (int z = m_dugDepth[x + 0*m_TexRes]; z >= 0; --z)
        m_digData [x + 0 * m_TexRes + z * m_TexRes * m_TexRes] = 255;

      for (int z = m_dugDepth[x + (m_TexRes-1)*m_TexRes]; z >= 0; --z)
        m_digData [x + (m_TexRes - 1) * m_TexRes + z * m_TexRes * m_TexRes] = 255;
    }

    for (int y = 0; y < m_TexRes; y++)
    {
      for (int z = m_dugDepth[0 + y*m_TexRes]; z >= 0; --z)
        m_digData [0 + y * m_TexRes + z * m_TexRes * m_TexRes] = 255;
      
      for (int z = m_dugDepth[(m_TexRes-1) + y*m_TexRes]; z >= 0; --z)
        m_digData [(m_TexRes - 1) + y * m_TexRes + z * m_TexRes * m_TexRes] = 255;
    }

    ////////////////////////////////////////////////////////////////////////
    // Air
    for (int x = 0; x < m_TexRes; x++)
      for (int y = 0; y < m_TexRes; y++)
        for (int z = m_dugDepth[x + 0*m_TexRes]+1; z < m_MaxDepth; z++)
          m_digData [x + y * m_TexRes + z * m_TexRes * m_TexRes] = 0;

    /*
    byte[] digDataCopy = new byte[m_digData.Length];
    for (int i = 0; i < m_digData.Length; i++)
      digDataCopy [i] = m_digData [i];
    */

    ////////////////////////////////////////////////////////////////////////
    /// Placing Stones
    float stonePercentage = 0.05f;
    int stoneCount = Mathf.FloorToInt((float)(m_TexRes * m_TexRes * m_MaxDepth) * stonePercentage);

    while (stoneCount > 0)
    {
      Vector3 stoneMid = new Vector3(
        Random.value * (m_TexRes - 2) + 1,
        Random.value * (m_TexRes - 2) + 1,
        Random.value * (m_MaxDepth * 0.75f - 2) + 1);

      for (float fx = -3.5f; fx < +3.6f; fx += 1.0f)
        for (float fy = -3.5f; fy < +3.6f; fy += 1.0f)
          for (float fz = -2.5f; fz < +2.6f; fz += 1.0f)
          {
            if ((Mathf.Abs(fx) + Mathf.Abs(fy) + Mathf.Abs(fz)) > 4.5f)
              continue;
            
            Vector3 offset = new Vector3(fx, fy, fz);
            Vector3 fPt = stoneMid + offset;

            int x = Mathf.FloorToInt(fPt.x);
            int y = Mathf.FloorToInt(fPt.y);
            int z = Mathf.FloorToInt(fPt.z);
        
            if ((x < 0) || (y < 0) || (z < 0) ||
              (x >= m_TexRes) || 
              (y >= m_TexRes) || 
              (z >= m_MaxDepth)) 
              continue;

            if (m_digData [x + y * m_TexRes + z * m_TexRes * m_TexRes] == 1)
            {
              m_digData [x + y * m_TexRes + z * m_TexRes * m_TexRes] = 2;
              stoneCount -= 1;
            }
      
            stoneCount -= 1;  // Safety Net
          }
    }

    ////////////////////////////////////////////////////////////////////////
    /// Placing Dino
    /// 
    {
      // Dino Stats
      m_dinoSurf = 0;
      m_dinoBlock = 0;
      m_dinoDestroyed = 0;
      m_dinoExposed = 0;

      // Dino Params
      int randDino = Random.Range(0, m_dinoList.Length - 1);
      Texture2D dinoTex = m_dinoList [randDino];
      bool flipX = (Random.value > 0.5);
      bool flipY = (Random.value > 0.5);
      int offX = Random.Range(0, m_TexRes - dinoTex.width - 1);
      int offY = Random.Range(0, m_TexRes - dinoTex.height - 1);
      int dinoDepth = Random.Range(5, Mathf.FloorToInt(m_MaxDepth * 0.7f));

      // Place Dino in Dirt
      Color32[] colBuf = dinoTex.GetPixels32();
      for (int x = 0; x < dinoTex.width; x++)
      {
        for (int y = 0; y < dinoTex.height; y++)
        {
          Color32 c = colBuf [x + y * dinoTex.width];
          int dH = ((c.r + c.g + c.b) * 6) / (255 * 3);

          bool isSurf = false;
          for (int z = Mathf.Max(1,dinoDepth-dH); z < (dinoDepth+dH); z++)
          {
            int ax = flipX ? (offX + dinoTex.width - x - 1) : (offX + x);
            int ay = flipY ? (offY + dinoTex.height - y - 1) : (offY + y);

            isSurf = true;
            m_digData [ax + ay * m_TexRes + dinoDepth * m_TexRes * m_TexRes] = 10;
            m_dinoBlock += 1;
          }

          if (isSurf)
            m_dinoSurf += 1;
        }
      }

      // Stats
      m_stats.m_dinoNumber = randDino;
      m_stats.m_dinoFlippedX = flipX;
      m_stats.m_dinoFlippedY = flipY;
      m_stats.m_dinoX = offX;
      m_stats.m_dinoY = offY;
      m_stats.m_dinoZ = dinoDepth;

      m_stats.m_dinoSurfArea = m_dinoSurf;
      m_stats.m_dinoBlocks = m_dinoBlock;
    }

    //////////////////////////////////////////////////////
    // Setup Damage Tex
    m_damageMap = new byte[m_TexRes * m_TexRes];
    for (int i = 0; i < m_damageMap.Length; i++)
      m_damageMap [i] = 50;

    //////////////////////////////////////////////////////
    /// Gui Setup
    GUIStyle gs = new GUIStyle();
    gs.border = new RectOffset(2, 2, 2, 2);
    gs.font = m_guiFont;
    gs.fontSize = 20;
    gs.alignment = TextAnchor.MiddleCenter;
    gs.normal.background = m_buttonBackTex;
    gs.normal.textColor = Color.grey; 
    gs.active.background = m_buttonBackTexActive;
    gs.active.textColor = Color.black;
    m_buttonStyle = gs;

    //////////////////////////////////////////////////////
    /// Startup
    m_startTime = Time.realtimeSinceStartup;
    m_xRayOn = false;

    m_gTrack = GameObject.FindGameObjectWithTag("GALog").GetComponent<GoogleTracker>();
    m_gTrack.PostEvent("Level", "Start", "Dino", m_stats.m_dinoNumber);

    m_IsSetup = true;
  }
	
  // Update is called once per frame
  void Update()
  {
    m_stats.m_timeTaken += Time.deltaTime;

    if (m_gTrack.m_timeSinceEvent > 60.0f)
    {
      m_gTrack.PostEvent("Level", "Tick", "dig", 
                         (1000 * m_stats.m_dinoExposed / m_stats.m_dinoSurfArea) +
        (m_stats.m_dinoDestroyed * 1000));
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// Mouse Handle
    Ray mr = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hitInfo;
    bool isHit = (!m_xRayOn) && m_boxCollider.Raycast(mr, out hitInfo, 10000.0f);

    Screen.showCursor = !isHit;
    if (m_brushObj.activeSelf != isHit)
      m_brushObj.SetActive(isHit);

    if (isHit)
    {
      m_brushObj.transform.position = hitInfo.point - mr.direction;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// Brush Controls
    if (Input.GetKeyUp(KeyCode.Equals))
    {
      m_brushStrength = Mathf.Max(0, m_brushStrength - 1);
    }

    if (Input.GetKeyUp(KeyCode.Minus))
    {
      m_brushStrength = Mathf.Min(m_toolStrength.Length - 1, m_brushStrength + 1);
    }

    if (Input.GetKeyUp(KeyCode.RightBracket))
    {
      m_brushID = Mathf.Max(m_brushID - 1, 0);
      UpdateBrush();
    }

    if (Input.GetKeyDown(KeyCode.LeftBracket))
    {
      m_brushID = Mathf.Min(m_brushID + 1, m_brush.Length - 1);
      UpdateBrush();
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// Dig
    if ((Input.GetMouseButton(0)) && isHit)
    {
      m_timSinceBrush += Time.deltaTime;

      if (Input.GetMouseButtonDown(0))
        m_timSinceBrush = 0.11f;

      if (m_timSinceBrush > (0.1f))
      {
        int hitX;
        int hitY;
        ConvertHitToPixCoords(hitInfo, out hitX, out hitY);

        // Get Current Colour
        BrushArea(hitX, hitY, m_timSinceBrush * m_brushStrVal [m_brushStrength], m_brush [m_brushID]);
        m_timSinceBrush = 0;
      }
    } else
    {
      for (int i = 0; i < m_damageMap.Length; i++)
      {
        switch (m_digData [i + m_dugDepth [i] * (m_TexRes * m_TexRes)])
        {
          case 2:
            m_damageMap [i] = 150;
            break;

          case 10:
            m_damageMap [i] = 200;
            break;

          case 255:
            m_damageMap [i] = 255;
            break;

          default:
            m_damageMap [i] = 40;
            break;
        }

      }
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// Xray
    if (m_xRayOn)
      m_stats.m_xRayTime += Time.deltaTime;

    if (Input.GetKeyDown(KeyCode.X))
      m_xRayOn = true;

    if (Input.GetKeyUp(KeyCode.X))
      m_xRayOn = false;

    ////////////////////////////////////////////////////////////////////////////////
    // Update GUI
    m_textTool.text = m_toolNames [m_brushID] + " " + m_toolStrength [m_brushStrength] + " Tool";
    m_textExposed.text = "Dino Found: <b>" + (100.0f * (float)m_dinoExposed / (float)m_dinoSurf).ToString("#.00") + "%</b>";
    m_textDamaged.text = "Damage: <b>" + (100.0f * (float)m_dinoDestroyed / (float)m_dinoBlock).ToString("#.00") + "%</b>";
  }

  void OnGUI()
  {
    switch (m_stats.m_gameMode)
    {
      case 0: // Practice
        {

          if (GUI.Button(new Rect(10, Screen.height - 40, 150, 30), "Finish Dig", m_buttonStyle))
          {
            m_gTrack.PostEvent("Level", "End", "dig",
                         (1000 * m_stats.m_dinoExposed / m_stats.m_dinoSurfArea) +
              (m_stats.m_dinoDestroyed * 1000));
            m_stats.SavePrefs();
            Application.LoadLevel("Score");
          }
        }
        break;

      case 1: // Timed
        {
          System.TimeSpan ts = System.TimeSpan.FromSeconds(m_timerLimit - m_stats.m_timeTaken);
          string resStr = "Time:" 
            + ts.Minutes.ToString("0") + "." 
            + ts.Seconds.ToString("00");

          GUI.Label(new Rect(10, Screen.height - 40, 150, 30), resStr, m_buttonStyle);

          if (m_stats.m_timeTaken > m_timerLimit)
          {
            m_stats.SavePrefs();
            Application.LoadLevel("Score");
          }
        }
        break;

      case 2: // Exposed
        {
          string resStr = "Progess: " + 
            (100 * m_stats.m_dinoExposed / m_stats.m_dinoSurfArea).ToString("#") + "%";

          GUI.Label(new Rect(10, Screen.height - 40, 150, 30), resStr, m_buttonStyle);
        
          if ((100 * m_stats.m_dinoExposed / m_stats.m_dinoSurfArea) > 90)
          {
            m_stats.SavePrefs();
            Application.LoadLevel("Score");
          }
        }
        break;
    }
  }

  void UpdateBrush()
  {
    m_brushObj.renderer.material.mainTexture = m_brushTemplate [m_brushID];
    m_brushObj.transform.localScale = new Vector3(m_brush [m_brushID].m_halfWidth * 2.0f, 1.0f, m_brush [m_brushID].m_halfHeight * 2.0f);
  }


  void ConvertHitToPixCoords(RaycastHit hitInfo, out int hitX, out int hitY)
  {
    Vector2 hitPointRel = new Vector2((hitInfo.point.x / m_boxCollider.size.x / transform.localScale.x) + 0.5f, 
                                      (hitInfo.point.z / m_boxCollider.size.z / transform.localScale.z) + 0.5f);
    hitX = Mathf.FloorToInt(hitPointRel.x * m_TexRes);
    hitY = Mathf.FloorToInt(hitPointRel.y * m_TexRes);
  }

  void BrushArea(int midX, int midY, float brushStrength, BrushTemplate brush)
  {
    // Safety Checks
    int startX = midX - brush.m_halfWidth;
    int startY = midY - brush.m_halfHeight;
    int pixWidth = brush.m_halfWidth * 2;

    int startOffset = startX + startY * m_TexRes;
    for (int i = 0; i < brush.m_strength.Length; i++)
    {
      // Get and Update Buffer
      int offsetIndex = startOffset + (i % pixWidth) + m_TexRes * (i / pixWidth);
      int x = startX + (i % pixWidth);
      int y = startY + (i / pixWidth);

      if ((x < 0) || (y < 0) || (x >= m_TexRes) || (y >= m_TexRes))
      {
        //  Do Nothing
      } else
      {
        if (m_damageMap [offsetIndex] < 255)
        {
          int digDataIndex = offsetIndex + m_dugDepth [offsetIndex] * m_TexRes * m_TexRes;
          int dmg = Mathf.FloorToInt(brush.m_strength [i] * brushStrength * 254) + (brush.m_strength [i] > 0 ? 1 : 0);

          if (m_damageMap [offsetIndex] <= dmg)
          {
            bool isDino = false;

            // Stats
            switch (m_digData [digDataIndex])
            {
              case 1:
                m_stats.m_dirtDug += 1;
                break;

              case 2:
                m_stats.m_stoneDug += 1;
                break;

              case 10:
                isDino = true;
                m_dinoDestroyed += 1;
                m_stats.m_dinoDestroyed += 1;
                break;
            }
              
            // Destroy and Update
            m_digData [digDataIndex] = 0;
            while ((m_dugDepth [offsetIndex]  > 0) && (m_digData [digDataIndex] == 0))
            {
              m_dugDepth [offsetIndex] -= 1;
              digDataIndex = offsetIndex + m_dugDepth [offsetIndex] * m_TexRes * m_TexRes;
            }


            switch (m_digData [offsetIndex])
            {
              case 1:
                m_damageMap [i] = 40;
                break;                
              case 2:
                m_damageMap [i] = 150;
                break;                
              case 10:
                m_damageMap [i] = 200;
                break;
              case 255:
                m_damageMap [i] = 255;
                break;
            }

            // Expose Dino
            if (!isDino && (m_digData [digDataIndex] == 10))
            {
              m_dinoExposed += 1;
              m_stats.m_dinoExposed += 1;
            }

          } else
          {
            m_damageMap [offsetIndex] -= (byte)dmg;
          }
        }

      }
    }

  }
}
