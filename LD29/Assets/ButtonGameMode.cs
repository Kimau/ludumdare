using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUIText))]
public class ButtonGameMode : MonoBehaviour
{
  public int m_gameMode;
  public string[] m_modeNames = {
        "Practice",
        "Timed",
        "Full Expose"
    };

  private GUIText m_guiText;

  void Start()
  {
    m_gameMode = 0;
    m_guiText = GetComponent<GUIText>();
    m_guiText.text = m_modeNames[m_gameMode];
    PlayerPrefs.SetInt("GameState gameMode", m_gameMode);
  }

  void Update()
  {
    // 
    if (Input.GetMouseButtonDown(0))
    {
      if (m_guiText.HitTest(Input.mousePosition))
      {
        m_gameMode = (m_gameMode + 1) % m_modeNames.Length;
        m_guiText.text = m_modeNames[m_gameMode];
        PlayerPrefs.SetInt("GameState gameMode", m_gameMode);
      }
    }
    else
    {
      if (m_guiText.HitTest(Input.mousePosition))
        m_guiText.color = Color.blue;
      else
        m_guiText.color = Color.white;
    }

  }
}
