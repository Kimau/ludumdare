using UnityEngine;
using System.Collections;

public class FinalScoreDisplay : MonoBehaviour 
{
	public TextMesh redScore;
	public TextMesh blueScore;
	public TextMesh yellowScore;
	public TextMesh purpleScore;
	public TextMesh greenScore;
	
	public TextMesh finalScore;
	
	public void Awake()
	{
		UpdareScores();
	}
	
	public void UpdareScores()
	{
		GenerateMine gMine = GameObject.FindWithTag("MineGen").GetComponent<GenerateMine>();
		
		if(gMine == null)
			Debug.LogError("Scoreboard cannot find the Mine Generator");
		
		redScore.text    = gMine.resLevels[MineBlock.RED_BLOCK].ToString("D3");
		blueScore.text   = gMine.resLevels[MineBlock.BLUE_BLOCK].ToString("D3");
		yellowScore.text = gMine.resLevels[MineBlock.YELLOW_BLOCK].ToString("D3");
		purpleScore.text = gMine.resLevels[MineBlock.PURPLE_BLOCK].ToString("D3");
		greenScore.text  = gMine.resLevels[MineBlock.GREEN_BLOCK].ToString("D3");
		
		finalScore.text  = gMine.score.ToString();
	}
}
