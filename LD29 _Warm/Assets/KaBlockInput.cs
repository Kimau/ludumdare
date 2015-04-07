using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KaBlockInput : MonoBehaviour
{
  public TextMesh m_inputText;
  public List<int> m_validOptions;
  public int m_corrAnswer;
  public int m_Col;
  public int m_Row;

  MeshRenderer meshR;
  bool m_isMouseOver;
  Coroutine m_fader;

  // Use this for initialization
  void Start()
  {
    m_isMouseOver = false;
    meshR = GetComponent<MeshRenderer>();
    m_corrAnswer = 1;	

    if (m_inputText)
      m_inputText.text = "";
  }
	
  // Update is called once per frame
  void Update()
  {

  }

  public void SetVal(int val)
  {
    if ((val > 0) && (val < 10))
      m_inputText.text = "" + val;
    else
      m_inputText.text = "";
  }

  public void MouseIn()
  {
    if (m_isMouseOver)
      return;

    StopCoroutine("FadeWhite");

    m_isMouseOver = true;
    meshR.material.color = Color.red;
  }

  public void MouseOut()
  {
    if (!m_isMouseOver)
      return;

    m_isMouseOver = false;
    StartCoroutine("FadeWhite", 0.3f);
  }

  public IEnumerator FadeWhite(float timeToFade)
  {
    float counter = timeToFade;
    while (counter > 0)
    {
      counter -= 0.1f;
      meshR.material.color = Color.Lerp(Color.white, Color.red, Mathf.Max(0, counter / timeToFade));
      yield return new WaitForSeconds(0.1f);
    }


  }
}
