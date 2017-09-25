using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUD : MonoBehaviour {

    public Level level;
    public GameEnd gameEnd; //游戏结束界面控制
    //UI索引
    //剩余量
    public Text remain_text;
    public Text remain_subtext;
    //目标分数
    public Text target_text;
    public Text target_subtext;
    //星星
    public Image[] stars;
    public Text score_text;

    private int starIdx = 0;

	// Use this for initialization
	void Start () {

        SetStars();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //设置星星的显示
    public void SetStars()
    {
        SetStars(starIdx);
    }
    public void SetStars(int idx)
    {
        starIdx = idx;
        for (int i = 0; i < stars.Length; i++)
        {
            if (i == starIdx)
                stars[i].enabled = true;
            else
                stars[i].enabled = false;
        }
    }
    //设置剩余量
    public void SetRemain(int remainNum)
    {
        remain_text.text = remainNum.ToString();
    }
    public void SetRemain(string time)
    {
        remain_text.text = time;
    }
    //设置目标
    public void SetTarget(int targeNum)
    {
        target_text.text = targeNum.ToString();
    }
    //设置当前得分
    public void SetScore(int score)
    {
        score_text.text = score.ToString();
        //匹配星星
        int currentStar = 0;
        if(score > level.score1Star)
        {
            currentStar = 1;
            if(score > level.score2Star)
            {
                currentStar = 2;
                if (score > level.score3Star)
                    currentStar = 3;
            }
        }
        SetStars(currentStar);
    }
    //设置关卡显示
    public void SetLevelText(Level.LevelType type)
    {
        switch (type)
        {
            case Level.LevelType.MOVES:
                remain_subtext.text = "remain steps";
                target_subtext.text = "targe score";
                break;
            case Level.LevelType.TIMER:
                remain_subtext.text = "remain time";
                target_subtext.text = "targe score";
                break;
            case Level.LevelType.OBSTACLE:
                remain_subtext.text = "remain steps";
                target_subtext.text = "remain bubble";
                break;
        }
    }
    public void OnGameWin(int score)
    {
        gameEnd.ShowWin(score, starIdx);
        //存储最高星星数
        string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if(starIdx >= PlayerPrefs.GetInt(scene, 0))
        {
            PlayerPrefs.SetInt(scene, starIdx);
            //Debug.Log("SAVE" + scene + ":" + starIdx);
        }
    }
    public void OnGameLose()
    {
		Debug.Log ("HUD gameover");
        gameEnd.ShowLose();
    }
}
