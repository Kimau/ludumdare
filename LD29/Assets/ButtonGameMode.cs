using UnityEngine;
using System.Collections;

public class ButtonGameMode : MonoBehaviour
{
  public int m_gameMode;
  public string[] m_modeNames = {
    "Practice",
    "Timed",
    "Full Expose"
  };
  
  // Use this for initialization
  void Start()
  {
    m_gameMode = 0;
    guiText.text = m_modeNames [m_gameMode];
    PlayerPrefs.SetInt("GameState gameMode", m_gameMode);
  }
  
  
  void Update()
  {
    // 
    if (Input.GetMouseButtonDown(0))
    {
      if (guiText.HitTest(Input.mousePosition))
      {
        m_gameMode = (m_gameMode + 1) % m_modeNames.Length;
        guiText.text = m_modeNames [m_gameMode];
        PlayerPrefs.SetInt("GameState gameMode", m_gameMode);
      }
    } else
    {
      if (guiText.HitTest(Input.mousePosition))
        guiText.color = Color.blue;
      else
        guiText.color = Color.white;
    }
    
  }
}
