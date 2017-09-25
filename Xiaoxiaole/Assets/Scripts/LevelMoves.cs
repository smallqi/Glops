using UnityEngine;
using System.Collections;
/// <summary>
/// 在规定步数内得到相应分数的关卡
/// </summary>
public class LevelMoves : Level {

    public int totalStepNum;
    public int targetScore;

    private int currentStepLeft;
	// Use this for initialization
	void Start () {
        
        currentStepLeft = totalStepNum;
        type = LevelType.MOVES;

        hud.SetLevelText(type);
        hud.SetRemain(totalStepNum);
        hud.SetTarget(targetScore);
        hud.SetScore(currentScore);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    //每次移动增加一次步数
    public override void OnMove()
    {
        currentStepLeft--;
		Debug.Log ("remain：" + currentStepLeft);
        hud.SetRemain(currentStepLeft);

        if(currentStepLeft == 0)  //所有步数用完，游戏结束
        {
            grid.GameOver();
			Debug.Log ("levelMoves gameover");
            if (currentScore >= targetScore)
                GameWin();
            else
                GameLose();
        }
    }
}
