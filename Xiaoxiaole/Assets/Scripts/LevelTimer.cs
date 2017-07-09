using UnityEngine;
using System.Collections;
/// <summary>
/// 指定时间内获得分数
/// </summary>
public class LevelTimer : Level {

    public float totalTime;   //总时间
    public int targeScore;  //目标分数

    private float leftTime;   //剩余时间
    private bool timeOver = false;  //时间结束

	// Use this for initialization
	void Start () {
        type = LevelType.TIMER;

        leftTime = totalTime;

        hud.SetLevelText(type);
        hud.SetRemain( string.Format("{0}:{1:00}", (int)leftTime / 60, (int)leftTime % 60) );
        hud.SetTarget(targeScore);
        hud.SetScore(currentScore);
    }
	
	// Update is called once per frame
	void Update () {
        if (!timeOver)
        {
            leftTime -= Time.deltaTime; //统计时间
            hud.SetRemain(string.Format("{0}:{1:00}",
                Mathf.Max( (int)leftTime / 60, 0 ), Mathf.Max( (int)leftTime % 60), 0 ) );
            if (leftTime < 0)
            {
                timeOver = true;
                grid.GameOver();
                if (currentScore >= targeScore)
                    GameWin();
                else
                    GameLose();
            }
        }
	}
}
