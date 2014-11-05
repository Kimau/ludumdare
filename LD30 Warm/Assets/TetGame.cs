using UnityEngine;
using System.Collections;

public class TetGame : MonoBehaviour {
  public TetPiece m_currPiece;

  public float m_minX = -3.0f;
  public float m_maxX = +2.5f;
  public float m_blockSize = 0.5f;
  public float m_tickTime = 1.0f;
  public float m_inputLag = 0.1f;
  Vector3 m_downVec;

  float m_currInputLag = 0.0f;

	// Use this for initialization
	void Start () 
  {
    m_downVec = m_blockSize * Vector3.down / m_tickTime;
	}
	
	// Update is called once per frame
	void Update () {
    if (m_currPiece)
    {
      m_currPiece.transform.position += m_downVec * Time.deltaTime;

      if(m_currInputLag > 0.0f)
        m_currInputLag -= Time.deltaTime;
      else
      {
        float hor = (Input.GetKey(KeyCode.RightArrow)?1f:0f) - (Input.GetKey(KeyCode.LeftArrow)?1f:0f);
        if(Mathf.Abs(hor) > 0.3f)
        {
          m_currPiece.transform.position += Vector3.right * m_blockSize * hor;
          m_currPiece.transform.position = new Vector3(
            Mathf.Clamp(m_currPiece.transform.position.x, m_minX, m_maxX),
            m_currPiece.transform.position.y,
            m_currPiece.transform.position.z);

          m_currInputLag = m_inputLag;
        }
      }
    }
	}
}
