using UnityEngine;
using System;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
  public GUIText m_TextDinoDiscover;
  public GUIText m_TextDinoDamaged;
  public GUIText m_TextTime;
  public GUIText m_TextXRay;
  public GUIText m_TextDugDirt;
  public GUIText m_TextDugStone;

  // Use this for initialization
  void Start()
  {
    Screen.showCursor = true;
    DiggableSurface.GameStats gs = DiggableSurface.GameStats.LoadPrefs();

    TimeSpan ts = TimeSpan.FromSeconds(gs.m_timeTaken);
    TimeSpan xts = TimeSpan.FromSeconds(gs.m_xRayTime);

    m_TextDinoDiscover.text = "<b>" + (100.0f * (float)gs.m_dinoExposed / (float)gs.m_dinoSurfArea).ToString("#.00") + "%</b>";
    m_TextDinoDamaged.text = "<b>" + (100.0f * (float)gs.m_dinoDestroyed / (float)gs.m_dinoBlocks).ToString("#.00") + "%</b>";
    m_TextTime.text = "<b>" 
      + ts.Hours + ":" 
      + ts.Minutes.ToString("00") + "." 
      + ts.Seconds.ToString("00") + "</b>";
    m_TextXRay.text = "<b>" 
      + xts.Hours + ":" 
      + xts.Minutes.ToString("00") + "." 
      + xts.Seconds.ToString("00") + "</b>";
    m_TextDugDirt.text = "<b>" + gs.m_dirtDug.ToString("N0") + "</b>";
    m_TextDugStone.text = "<b>" + gs.m_stoneDug.ToString("N0") + "</b>"; 
  }
	
  // Update is called once per frame
  void Update()
  {

    if (Input.GetMouseButtonDown(0))
    {
      if (guiText.HitTest(Input.mousePosition))
        Application.LoadLevel("Title");
    }
	
  }
}
