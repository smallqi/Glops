using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour {
    //自身的坐标
    private int x;
    private int y;
    public int X
    {
        get{ return x; }
        set { if (isMoveable) x = value; }
    }
    public int Y
    {
        get { return y; }
        set { if (isMoveable) y = value; }
    }
    //自身的类型
    private Grid.PieceType type;
    public Grid.PieceType Type
    {
        get { return type; }
    }
    //控制器
    private Grid grid;
    public Grid GridRef
    {
        get { return grid; }
    }
    //移动
    private MoveablePiece moveableComponent;
    public MoveablePiece MoveableComponent
    {
        get { return moveableComponent; }
    }
    public bool isMoveable = false;
    //颜色变换
    private ColorPiece colorComponent;
    public ColorPiece ColorComponent
    {
        get { return colorComponent; }
    }
    public bool isColor = false;
    //消除
    private ClearablePiece clearComponent;
    public ClearablePiece ClearComponent
    {
        get { return clearComponent; }
    }
    public bool isClear = false;
    //物体所值分数
    public int score;

    private void Awake()
    {
        //若有移动脚本说明可以移动
        moveableComponent = this.GetComponent<MoveablePiece>();
        if (moveableComponent != null)
            isMoveable = true;
        //有颜色组件说明可以变换颜色
        colorComponent = this.GetComponent<ColorPiece>();
        if (colorComponent != null)
            isColor = true;
        //有消除组件说明可以消除
        clearComponent = GetComponent<ClearablePiece>();
        if (clearComponent != null)
            isClear = true;
    }
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    //初始化piece，设置属性
    public void Init(int _x, int _y, Grid _grid,Grid.PieceType _type)
    {
        x = _x;
        y = _y;
        grid = _grid;
        type = _type;
    }

    private void OnMouseDown()
    {
        //if(grid.PieceA == null)
            grid.SetPieceA(this);
        /*
        else
        {
            grid.SetPieceB(this);
            grid.Change();
        }
        */
    }
    private void OnMouseEnter()
    {
        grid.SetPieceB(this);
    }
    private void OnMouseUp()
    {
        grid.Change();
    }
}
