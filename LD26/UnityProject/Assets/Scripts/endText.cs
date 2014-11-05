using UnityEngine;
using System.Collections;

public class endText : MonoBehaviour 
{
	TextMesh textMeshRef;
	
	void Start()
	{
		textMeshRef = GetComponent<TextMesh>();
		textMeshRef.text = "Hello World\n or should I say hello Ludum Dare \n Did you enjoy the game? \n Yes    No";
	}
	
	void ClearRenderChildren ()
	{
		Renderer[] children = GetComponentsInChildren<Renderer>();
		foreach (Renderer item in children)
			if(item.gameObject != gameObject)
				GameObject.Destroy(item.gameObject);
	}
	
	public void EnjoyNo()
	{
		ClearRenderChildren();
		textMeshRef.text = "Thanks\n For finishing\n It means a lot you played\n ^_^";
	}
	
	public void EnjoyYes()
	{
		ClearRenderChildren();
		textMeshRef.text = "Thanks!\n I found the minimal theme\n was restrictive but fun.\n I threw away some visuals and sounds\n because they didn't fit\n At one stage I started putting shooting in\n but there is enough violence... \nalso I started with the idea of a cube\n stealing away a circle for instant story\n but the damsel trope bothers me...\n I wanted to share this.\n Thanks for playing\n It's Everything!";
	}
}
