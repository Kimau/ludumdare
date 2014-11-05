using UnityEngine;
using System.Collections;

public class ButtonNewGame : MonoBehaviour
{
  // Use this for initialization
  void Start()
  {
    Screen.showCursor = true;
  }
	
	
  void Update()
  {
    // 
    if (Input.GetMouseButtonDown(0))
    {
      if (guiText.HitTest(Input.mousePosition))
        Application.LoadLevel("DigScene");
    } else
    {
      if (guiText.HitTest(Input.mousePosition))
        guiText.color = Color.blue;
      else
        guiText.color = Color.white;
    }

  }
}
