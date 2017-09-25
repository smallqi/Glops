using UnityEngine;
using System.Collections;
/// <summary>
/// 消除一定数量指定类型关卡
/// </summary>
public class LevelObstacle : Level {

    public int totalStepNum;    //总共的步数
    [System.Serializable]
    public struct PieceTypeNum  //需要消除的各类型个数,需要在grid的initial上进行配合
    {
        public Grid.PieceType type;
        public int num;

        private int left;   //剩余个数
        public int Left
        {
            get { return left; }
            set {
                if (value < 0)
                    return;
                left = value;
           }
        }
    }
    public PieceTypeNum[] pieceTypeNum;

    private int leftStep;   //剩余步数

	// Use this for initialization
	void Start () {
        type = LevelType.OBSTACLE;
        //赋上初始值
        leftStep = totalStepNum;
        for (int i = 0; i < pieceTypeNum.Length; i++)
            pieceTypeNum[i].Left = pieceTypeNum[i].num;

        hud.SetLevelText(type);
        hud.SetRemain(totalStepNum);
        hud.SetTarget(pieceTypeNum[0].num);
        hud.SetScore(currentScore);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    //移动步数
    public override void OnMove()
    {
        leftStep--;
        hud.SetRemain(leftStep);
        //Debug.Log("remanStep:" + leftStep);

        if (leftStep == 0)  //游戏结束
        {
            grid.GameOver();
			Debug.Log ("GameOver");
            if (isWin())
                GameWin();  //在OnPieceCleared输出一遍，这里再输出一遍
            else
                GameLose();
        }
    }
    //检测消除的元素是否为规定元素
    public override void OnPieceCleared(GamePiece piece)
    {
        base.OnPieceCleared(piece);
        for(int i=0; i<pieceTypeNum.Length; i++)
            if(pieceTypeNum[i].type == piece.Type)  //两者类型相同，不是sprite相同
            {
                pieceTypeNum[i].Left--;
                hud.SetTarget(pieceTypeNum[0].Left);
                //Debug.Log(pieceTypeNum[i].type + "remain:" + pieceTypeNum[i].Left);
                //如果步数有剩，但全部消除了，立即获得胜利
                if (pieceTypeNum[i].Left == 0)
                    if(isWin())
                    {
                        grid.GameOver();
                        GameWin();
                    }
                break;
            }

    }
    //是否获胜
    bool isWin()
    {
        int i;
        for (i = 0; i < pieceTypeNum.Length; i++)
            if (pieceTypeNum[i].Left != 0)
                return false;
        return true;
    }
}
