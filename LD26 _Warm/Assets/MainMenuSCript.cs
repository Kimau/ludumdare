using UnityEngine;
using System.Collections;

public class MainMenuSCript : MonoBehaviour 
{
	public GameObject howToSection;
	public GameObject buttonSection;
	public GameObject scoresSection;
	
	enum menuMode
	{
		main,
		how,
		score
	};

	void Start () 
	{
		SwitchMenu(menuMode.main);
	}
	
	public void StartNewGame()
	{
		Application.LoadLevel("gameLevel");
	}
	
	public void GotoScores()
	{
		SwitchMenu(menuMode.score);
	}
	
	public void GotoMain()
	{
		SwitchMenu(menuMode.main);
	}
	
	public void GotoHowTo()
	{
		SwitchMenu(menuMode.how);
	}
	
	void SwitchMenu(menuMode newMode)
	{
		switch(newMode)
		{
		case menuMode.main:
			buttonSection.SetActive(true);
			howToSection.SetActive(false);
			scoresSection.SetActive(false);
			break;
			
		case menuMode.how:
			buttonSection.SetActive(false);
			howToSection.SetActive(true);
			scoresSection.SetActive(false);
			break;
			
		case menuMode.score:
			buttonSection.SetActive(false);
			howToSection.SetActive(false);
			scoresSection.SetActive(true);
			break;
		}
	}
}
