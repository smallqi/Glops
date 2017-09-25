using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {
    //关卡类型
    public enum LevelType
    {
        TIMER,
        OBSTACLE,
        MOVES,
    }
    public Grid grid;   //游戏逻辑交互控制
    public HUD hud; //游戏运行界面控制

    //分数的星星等级
    public int score1Star;
    public int score2Star;
    public int score3Star;
    //当前关卡类型
    protected LevelType type;
    public LevelType Type
    {
        get { return type; }
    }
    //当前关卡分数
    protected int currentScore = 0;
    protected bool isGameWin = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //获胜
    public virtual void GameWin()
    {
        grid.GameOver();
        isGameWin = true;
        //StartCoroutine(ShowGameOver());

        while (grid.IsFill)
            Debug.Log("wait");
        //填充完毕则显示
        if (isGameWin)
            hud.OnGameWin(currentScore);
        else
            hud.OnGameLose();
    }
    //失败
    public virtual void GameLose()
    {
        grid.GameOver();
        isGameWin = false;
        StartCoroutine(ShowGameOver());
    }
    //
    public virtual void OnMove()
    {
        //to be override
    }
    //更新分数
    public virtual void OnPieceCleared(GamePiece piece)
    {
        currentScore += piece.score;
        hud.SetScore(currentScore);
        //Debug.Log("now score is " + currentScore);
    }
    //等待填充完毕后结束游戏
    protected IEnumerator ShowGameOver()
    {
        while (grid.IsFill)
            yield return 0;
        //填充完毕则显示
		if (isGameWin)
			hud.OnGameWin (currentScore);
		else {
			Debug.Log ("level gameover");
			hud.OnGameLose();
		}
           
    }
}
