using UnityEngine;
using System.Collections;

public class KaBlockDirection : MonoBehaviour
{
  public enum BlockType
  {
    BlockRight,
    BlockDown,
    BlockBoth
  }

  public BlockType m_bType;
  public TextMesh m_rightText;
  public TextMesh m_downText;

  // Use this for initialization
  void Start()
  {
	
  }
	
  // Update is called once per frame
  void Update()
  {
	
  }

  public void SetDown(int val)
  {
    if (val < 10)
      m_downText.text = val + ".";
    else
      m_downText.text = "" + val;
  }

  public void SetRight(int val)
  {
    if (val < 10)
      m_rightText.text = " " + val;
    else
      m_rightText.text = "" + val;
  }
}
