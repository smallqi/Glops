using UnityEngine;
using System.Collections;

public class MoveablePiece : MonoBehaviour {
    //获取piece属性信息
    private GamePiece gamePiece;

    //协同函数
    private IEnumerator moveCoroutine;

    void Awake()
    {
        gamePiece = this.GetComponent<GamePiece>();
    }
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    //管理协同程序
    public void move(int newX, int newY, float time = 0.1f)
    {
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
    }
    //控制逐渐下落
    IEnumerator MoveCoroutine(int x, int y, float time)
    {
        gamePiece.X = x;
        gamePiece.Y = y;
        Vector3 startPosition = this.transform.position;
        Vector3 stopPosition = gamePiece.GridRef.positionTrans(gamePiece.X, gamePiece.Y);
        for(float t=0; t < time; t += Time.deltaTime)
        {
            //用时间慢慢产生过渡
            this.transform.position = Vector3.Lerp(startPosition, stopPosition, t / time);
            yield return new WaitForSeconds(0f);
        }
        this.transform.position = stopPosition;
        //yield return new WaitForSeconds(time);
    }
}
