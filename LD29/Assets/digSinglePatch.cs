using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(MeshFilter))]
public class digSinglePatch : MonoBehaviour
{
  public DiggableSurface m_digger;

  // 
  Color32[] m_colBuffer;
  Texture2D m_surfaceTex;

  bool m_isSetup;

  // Cached
  Renderer m_renderer;

  void Start()
  {
    m_isSetup = false;

    m_renderer = GetComponent<Renderer>();

    if (m_digger == null)
      Debug.LogError("Unable to find Dig");

    if (m_digger.m_IsSetup)
      SetupSurface();
  }

  // Setup Surface Texture
  public void SetupSurface()
  {
    m_colBuffer = new Color32[m_digger.m_TexRes * m_digger.m_TexRes];
    m_surfaceTex = new Texture2D(m_digger.m_TexRes, m_digger.m_TexRes, TextureFormat.ARGB32, false);
    m_surfaceTex.filterMode = FilterMode.Point;

    m_renderer.material.mainTexture = m_surfaceTex;
  }

  void RebuildSurfaceTex()
  {
    // Build 2D Texture from Depth Data
    for (int i = 0; i < m_colBuffer.Length; i++)
      UpdateSurfacePoint(i);

    m_surfaceTex.SetPixels32(m_colBuffer);
    m_surfaceTex.Apply(false);
  }

  // 0.5f  -- 10
  void XRaySurfaceTex(float dirtNoise, float rockNoise)
  {
    for (int i = 0; i < m_colBuffer.Length; i++)
      XRaySurfacePoint(i, dirtNoise, rockNoise);

    m_surfaceTex.SetPixels32(m_colBuffer);
    m_surfaceTex.Apply();
  }

  void XRaySurfacePoint(int i, float dirtNoise, float rockNoise)
  {
    m_colBuffer[i] = new Color32(0, 0, 0, 0);
    int z = m_digger.Depth(i);
    if (z == 0)
    {
      return;
    }

    float noise = 0.0f;
    float depRecip = 1.0f / 254.0f;
    bool isDino = false;
    while (!isDino && z > 0)
    {
      switch (m_digger.Data(i + z * (m_digger.m_TexRes * m_digger.m_TexRes)))
      {
        case 1:
          noise += dirtNoise / depRecip;
          break;
        case 2:
          noise += rockNoise / depRecip;
          break;
        case 10:
          isDino = true;
          break;
      }

      z -= 1;
    }

    noise = Mathf.Min(1.0f, noise);

    byte res;
    if (isDino)
      res = (byte)(Mathf.Max(0.0f, 1.0f - Random.value * noise) * 255.0f);
    else
      res = (byte)(Mathf.Min(1.0f, 0.0f + Random.value * noise) * 255.0f);

    m_colBuffer[i].r = res;
    m_colBuffer[i].g = res;
    m_colBuffer[i].b = res;
    m_colBuffer[i].a = 255;
  }


  void UpdateSurfacePoint(int i)
  {
    byte z = m_digger.Depth(i);
    if (z == 0)
    {
      m_colBuffer[i] = new Color32(0, 0, 0, 0);
      return;
    }

    m_colBuffer[i].a = z;
    float alt = 1.0f - z / (m_digger.m_MaxDepth + 0.1f);
    switch (m_digger.Data(i + z * (m_digger.m_TexRes * m_digger.m_TexRes)))
    {
      case 1:
        if (z > (m_digger.m_MaxDepth - 4))
        {
          m_colBuffer[i].r = 27;
          m_colBuffer[i].g = 110;
          m_colBuffer[i].b = 27;
        }
        else
        {
          m_colBuffer[i].r = (byte)Mathf.FloorToInt(Mathf.Lerp(225, 69, alt));
          m_colBuffer[i].g = (byte)Mathf.FloorToInt(Mathf.Lerp(189, 32, alt));
          m_colBuffer[i].b = (byte)Mathf.FloorToInt(Mathf.Lerp(119, 18, alt));
        }
        break;

      case 2:
        m_colBuffer[i].r = (byte)Mathf.FloorToInt(Mathf.Lerp(160, 64, alt));
        m_colBuffer[i].g = (byte)Mathf.FloorToInt(Mathf.Lerp(160, 64, alt));
        m_colBuffer[i].b = (byte)Mathf.FloorToInt(Mathf.Lerp(160, 64, alt));
        break;

      case 10:
        m_colBuffer[i].r = 100;
        m_colBuffer[i].g = 100;
        m_colBuffer[i].b = 250;
        break;

      case 255:
        m_colBuffer[i].r = 0;
        m_colBuffer[i].g = 0;
        m_colBuffer[i].b = 0;
        break;
    }
  }

  /** ---------------------------------------------------------------------
   *               UPDATE 
   *-----------------------------------------------------------------------*/
  void Update()
  {
    //Setup if Dig is Setup
    if (!m_isSetup)
    {
      if (m_digger.m_IsSetup)
        SetupSurface();
      else
        return;
    }

    if (m_digger.m_xRayOn)
      XRaySurfaceTex(0.00002f, 0.05f);
    else
      RebuildSurfaceTex();
  }
}
