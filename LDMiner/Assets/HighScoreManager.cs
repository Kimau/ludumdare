using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;

public class HighScoreManager : MonoBehaviour 
{
	CookieContainer myCookies;	
	public string host = "localhost";
	public string port = "10634";
	
	TextMesh tMesh;
	
	void Awake()
	{
		tMesh = GetComponentInChildren<TextMesh>();
	}
	
	void OnEnable()
	{
		myCookies = new CookieContainer();
		
		if(myCookies.Count < 1)
			GetAuthorityCookiesNOW();
		
		if(myCookies.Count > 0)
			GetScores();
		else
			Debug.LogWarning("No Cookies :(");
	}
		
	void GetAuthorityCookiesNOW()
	{
		string reqString = "http://" + host + ":" + port + "/_ah/login?action=Login&continue=http%3A%2F%2F" + host + "%3A" + port + "%2F";
		reqString += "&email=" + "boom%40example.com";
		
		HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(reqString);
		webReq.Referer = "http://" + host + ":" + port + "/_ah/login";
		webReq.UserAgent = "Mozilla/5.0";
		webReq.CookieContainer = myCookies;
		
		HttpWebResponse webRep = (HttpWebResponse)webReq.GetResponse();
		if(webRep.StatusCode != HttpStatusCode.OK)
			Debug.LogError(webRep.StatusCode + "\n" + webRep.StatusDescription);
	}
	
	
	string ColourToHexStr(Color inF)
	{
		string res = "#" + 
			((int)(inF.r * 255.0f)).ToString("X2") +
			((int)(inF.g * 255.0f)).ToString("X2") +
			((int)(inF.b * 255.0f)).ToString("X2") +
				"FF";
		return res;
	}
	
	void GetScores()
	{
		HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create("http://" + host + ":" + port + "/");
		webReq.Referer = "http://" + host + ":" + port + "/";
		webReq.UserAgent = "Mozilla/5.0";
		
		// webReq.Headers.Add("aid", a.ToString());
		
		webReq.CookieContainer = myCookies;
		webReq.Method = "GET";
		
		HttpWebResponse webRep = (HttpWebResponse)webReq.GetResponse();
		
		// Validate it's what we expect
		/*
		System.Diagnostics.Debug.Assert((webRep.StatusCode == HttpStatusCode.OK), webRep.StatusCode + "\n" + webRep.StatusDescription);
		System.Diagnostics.Debug.Assert(webRep.ContentType.Contains("application/qubed;"), "Content Type does not match: " + webRep.ContentType.ToString());
		*/
	
		// All Good
		Stream stream = webRep.GetResponseStream();
		StreamReader contentStream = new StreamReader(stream);
		string contentText = contentStream.ReadToEnd();
		stream.Close();
		
		// Tokenize
		string[] lines = contentText.Split('\n');
		
		string finalString = "";
		foreach(string scoreLine in lines)
		{			
			string[] stubs = scoreLine.Split(',');
			
			// Get Name
			finalString += "<b>" + stubs[0] + "</b>";
			finalString += "\t" + stubs[1].Split(':')[1];
			finalString += "\t <color=#FF0000FF>" + stubs[2].Split(':')[1] + "</color>";
			finalString += "\t <color=#FFFF00FF>" + stubs[5].Split(':')[1] + "</color>";
			finalString += "\t <color=#00FF00FF>" + stubs[4].Split(':')[1] + "</color>";			
			finalString += "\t <color=#00FFFFFF>" + stubs[6].Split(':')[1] + "</color>";
			finalString += "\t <color=#0000FFFF>" + stubs[3].Split(':')[1] + "</color>";
			finalString += "\n";
		}
		
		if(tMesh)
		{
			tMesh.text = finalString;
		}
		
		Debug.Log(contentText);
	}
}
