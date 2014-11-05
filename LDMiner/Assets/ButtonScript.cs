using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ButtonScript : MonoBehaviour 
{
	public string ActionOnPress = "Something";
	public int textIndex = 0;
	public GameObject textObject;
	
	bool pressed = false;
	Camera uiCam;

	void Start () 
	{
		if(textObject)
			textObject.renderer.material.mainTextureOffset = new Vector2(0, 1.01f - 0.3333f * textIndex);
		
		uiCam = GameObject.FindGameObjectWithTag("uiCam").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		Ray mouseRay = uiCam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		
		if(collider.Raycast(mouseRay, out hitInfo, 1000.0f))
		{
			if(Input.GetMouseButtonDown(0))
				pressed = true;
			
			if((Input.GetMouseButtonUp(0)) && pressed)
			{
				pressed = false;
				uiCam.SendMessage(ActionOnPress);
			}
				
			renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
		}
		else
		{
			pressed = false;
			renderer.material.mainTextureOffset = new Vector2(0.0f, 0.5f);			
		}
		
		// Fallback
		if(Input.GetMouseButtonUp(0))
				pressed = false;
	}
}
