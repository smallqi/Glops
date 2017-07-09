using UnityEngine;
using System.Collections;

public class LevelSelect : MonoBehaviour {

    //保存3个按钮的实体
    public GameObject[] level;
	
    // Use this for initialization
	void Start () {
	    //初始化星星数显示
        for(int i=0; i<level.Length; i++)
        {
            int starsNum = PlayerPrefs.GetInt(level[i].name, 0);
            //Debug.Log("get:" + level[i].name + ":" + starsNum);
            for(int j=1; j<=starsNum; j++)
            {
                //获得星星
                Transform starTran = level[i].transform.FindChild("star" + j);
                if (starTran)
                {
                    starTran.gameObject.SetActive(true);
                    //Debug.Log("show:" + starTran.name);
                }
                    
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnLevelClick(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
