using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameEnd : MonoBehaviour {

    //显示
    public GameObject end;
    public GameObject score;

    public Text loseText;
    public Text scoreText;
    public Image[] stars;

	// Use this for initialization
	void Start () {
        //一开始不出现
        end.SetActive(false);
        for (int i = 0; i < stars.Length; i++)
            stars[i].enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    //游戏失败
    public void ShowLose()
    {
        end.SetActive(true);
        score.SetActive(false);

        //播放动画
        Animator animator = GetComponent<Animator>();
        if (animator)
            animator.Play("GameOver");
    }
    //游戏胜利
    public void ShowWin(int score, int starsNum)
    {
        end.SetActive(true);
        loseText.enabled = false;

        Animator animator = GetComponent<Animator>();
        if (animator)
            animator.Play("GameOver");

        scoreText.text = score.ToString();  //显示得分
        scoreText.enabled = false;  //星星显示完毕才显示
        StartCoroutine( ShowStarCoroutine(starsNum) );//显示星星

    }
    //显示星星
    private IEnumerator ShowStarCoroutine(int starsNum)
    {
        yield return new WaitForSeconds(1.0f);
        if (starsNum < stars.Length)
        {
            for(int i=0; i<=starsNum; i++)
            {
                //间隔0.5s逐个显示动画
                stars[i].enabled = true;
                yield return new WaitForSeconds(0.5f);
                if (i != starsNum)
                    stars[i].enabled = false;
            }
            scoreText.enabled = true;
        }
    }
    //重新载入
    public void OnRetryClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //重载当前场景
    }
    //返回主界面
    public void OnBackClick()
    {
        SceneManager.LoadScene("LevelSelect");
    }
}
