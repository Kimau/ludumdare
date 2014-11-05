using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;

public class GoogleTracker : MonoBehaviour
{  
  public string m_TrackingID;
  public string m_clientID;
  public int m_version;
  public string m_userID;
  public string m_appName;
  public float m_timeSinceEvent;
  
  // Use this for initialization
  void Start()
  {
    m_timeSinceEvent = 0.0f;
    m_version = 1;

    if (PlayerPrefs.HasKey("GAUserID"))
      m_userID = PlayerPrefs.GetString("GAUserID");
    else
    {
      m_userID = 
      Random.Range(Mathf.Pow(16, 7) + 1, Mathf.Pow(16, 8) - 1) + "-" +
        Random.Range(Mathf.Pow(16, 3) + 1, Mathf.Pow(16, 4) - 1) + "-" +
        Random.Range(Mathf.Pow(16, 3) + 1, Mathf.Pow(16, 4) - 1) + "-" +
        Random.Range(Mathf.Pow(16, 3) + 1, Mathf.Pow(16, 4) - 1) + "-" +
        Random.Range(Mathf.Pow(16, 5) + 1, Mathf.Pow(16, 6) - 1) + 
        Random.Range(Mathf.Pow(16, 5) + 1, Mathf.Pow(16, 6) - 1);

      PlayerPrefs.SetString("GAUserID", m_userID);
    }
  }
  
  // Update is called once per frame
  void Update()
  {
    m_timeSinceEvent += Time.deltaTime;
  }
    
  public void PostEvent(string eventCat, string eventAct, string eventLabel, int eventVal)
  {
    m_timeSinceEvent = 0;

    string postData = "v=1";
    postData += "&tid=" + m_TrackingID;
    postData += "&cid=" + m_clientID;
    //postData += "&uid=" + m_userID;
    postData += "&an=" + WWW.EscapeURL(m_appName);
    postData += "&t=event";
    postData += "&sr=" + Screen.width + "x" + Screen.height;
    postData += "&ec=" + eventCat;
    postData += "&ea=" + eventAct;
    postData += "&el=" + eventLabel;
    postData += "&ev=" + eventVal;

    StartCoroutine(PostAsync("http://www.google-analytics.com/collect", postData));
  }

  IEnumerator PostAsync(string url, string postData)
  {
    HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);
    webReq.Referer = url;
    webReq.UserAgent = "Mozilla/5.0";
    webReq.Method = "POST";
    webReq.ContentType = "application/x-www-form-urlencoded";
    
    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
    webReq.ContentLength = byteArray.Length;
    
    Stream dataStream = webReq.GetRequestStream();
    dataStream.Write(byteArray, 0, byteArray.Length);
    dataStream.Close();

    WebResponse response = null;
    StreamReader reader = null;
    try
    {
      response = webReq.GetResponse();
      // Debug.Log(((HttpWebResponse)response).StatusDescription);
      dataStream = response.GetResponseStream();
      reader = new StreamReader(dataStream);
      string responseFromServer = reader.ReadToEnd();
    } finally
    {
      // Debug.Log(responseFromServer);
      if (reader != null)
        reader.Close();
      if (dataStream != null)
        dataStream.Close();
      if (response != null)
        response.Close();
    }


    yield return null;
  }
}
