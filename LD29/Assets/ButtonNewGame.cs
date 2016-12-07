using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GUIText))]
public class ButtonNewGame : MonoBehaviour
{
  private GUIText m_guiText;

  void Start()
  {
    Cursor.visible = true;
    m_guiText = GetComponent<GUIText>();
  }

  void Update()
  {
    // 
    if (Input.GetMouseButtonDown(0))
    {
      if (m_guiText.HitTest(Input.mousePosition))
        SceneManager.LoadScene("DigScene", LoadSceneMode.Single);
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
