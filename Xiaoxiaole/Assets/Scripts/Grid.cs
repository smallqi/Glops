using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {
    //鱼儿场景背景
    public GameObject bgPrefab;
    //控制网格的长和宽
    public int xdim;
    public int ydim;
    //控制生成网格的偏移量
    public float xIndex = -1.0f;
    public float yIndex = -3.0f;
    //记录鱼的类别
    public enum PieceType
    {
        EMPTY,
        NORMOL,
        BUBBLE,
        ROW_CLEAR,
        COLUMN_CLEAR,
        COLOR_CLEAR,
        COUNT
    }
    //记录属性与OBJ
    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    }
    public PiecePrefab[] piecePrefab;   //记录了不同种类和与之相对应的prefab
                                        //长度由场景添加直接决定
    //记录关卡初始设置
    [System.Serializable]
    public struct PiecePostion
    {
        public PieceType type;
        public int x;
        public int y;
    }
    public PiecePostion[] initialPieces;

    public float dropSpeed = 0.1f;   //下降时间

    //与view层的通信
    private GamePiece[,] gamePieces;   //鱼儿实体
    //索引
    private Dictionary<PieceType, GameObject> piecePrefabDic;
    //交换
    private GamePiece pieceA;
    private GamePiece pieceB;
    public GamePiece PieceA
    {
        get { return pieceA; }
        //set { pieceA = PieceA; }
    }
    public GamePiece PieceB
    {
        get { return pieceB; }
        //set { pieceB = PieceB; }
    }
    //与model层的通信
    public Level level;
    private bool gameOver = false;
    //是否正在填充，用来确定游戏结束时间
    private bool isFill;
    public bool IsFill
    {
        get { return isFill; }
    }

    // Use this for initialization
    void Awake () {
        piecePrefabDic = new Dictionary<PieceType, GameObject>();
        //加入索引表
        for (int i = 0; i < piecePrefab.Length; i++)
            if (!piecePrefabDic.ContainsKey(piecePrefab[i].type))
            {
                //Debug.Log("dic " + piecePrefab[i].type + " add");
                piecePrefabDic.Add(piecePrefab[i].type, piecePrefab[i].prefab);               
            }
        //生成背景表格
        for (int i=0; i<xdim; i++)
            for(int j=0; j<ydim; j++)
            { 
                GameObject pieceBg = Instantiate(bgPrefab, positionTrans(i, j, 0), Quaternion.identity) as GameObject;
                pieceBg.transform.parent = this.transform;  //设为Grid的孩子
            }

        gamePieces = new GamePiece[xdim, ydim];
        //生成初始的固定位置
        SpawnInitialPiece();
        //生成鱼儿
        StartCoroutine(Fill());
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    //位置偏移
    public Vector3 positionTrans(float x, float y, float z=0)
    {
        return new Vector3(x + xIndex, y + yIndex, z);
    }
    //填充
    IEnumerator Fill()
    {
        //bool refill = true;
        isFill = true;
        do
        {
            //Debug.Log("do fill");
            yield return new WaitForSeconds(dropSpeed);
            //当没有新填充时，则说明填充完成
            while (FillStep())
                yield return new WaitForSeconds(dropSpeed);
            //填充完毕后需要检查消除
        } while (ClearAllMatch());

        //yield return new WaitForSeconds(dropSpeed);
        //ClearAllMatch();
        isFill = false;
    }
    //一个一个填充
    bool FillStep()
    {
        bool isRefresh = false;
        //从下往上，第二层到最顶层
            //以行遍历，即x
        for(int y = 1; y < ydim; y++)
            for(int x=0; x < xdim; x++)
            {
                GamePiece aimPiece = gamePieces[x, y];
                GamePiece belowPiece = gamePieces[x, y - 1];
                //如果当前不为空且可以移动，则进行移动
                if(aimPiece != null && aimPiece.isMoveable)
                {
                    //竖直下为空则直接进行下落
                    if(belowPiece == null)
                    {
                        aimPiece.MoveableComponent.move(x, y - 1, dropSpeed);  //显示位置移动
                        gamePieces[x, y - 1] = aimPiece;    //坐标位置更新
                        gamePieces[x, y] = null;    //坐标位置更新
                        isRefresh = true;
                    }        
                }
            }
        //顶层再处理
        for(int x = 0; x < xdim; x++)
        {
            int y = ydim - 1;   //最顶层
            GamePiece aimPiece = gamePieces[x, y];
            if (aimPiece == null)
            {
                aimPiece = SpawnNewPiece(x, y, PieceType.NORMOL);   //新生成
                if(aimPiece.isColor)
                    aimPiece.ColorComponent.setColor((ColorPiece.ColorType)Random.Range(0, 6)); //随机颜色设置
                isRefresh = true;
            }
        }
        if (isRefresh)
            return isRefresh;
        //如果竖直下落全都无法进行了，则尝试斜线下落
        for (int x = 0; x < xdim; x++)
            for (int y = 1; y < ydim; y++)
            {
                GamePiece aimPiece = gamePieces[x, y];
                GamePiece belowPiece = gamePieces[x, y - 1];
                //如果当前不为空且可以移动，则进行移动
                if (aimPiece != null && aimPiece.isMoveable)
                {
                    //竖直下为空则直接进行下落
                    if (belowPiece == null)
                    {
                        aimPiece.MoveableComponent.move(x, y - 1, dropSpeed);  //显示位置移动
                        gamePieces[x, y - 1] = aimPiece;    //坐标位置更新
                        gamePieces[x, y] = null;    //坐标位置更新
                        isRefresh = true;
                    }
                    else//采取斜线下降
                    {
                        //先尝试左下，失败则再尝试右下，-1为左，1为右
                        for (int diag = -1; diag <= 1; diag += 2)
                        {
                            int diagX = x + diag;
                            int diagY = y - 1;
                            //防止越界
                            if (diagX >= 0 && diagX < xdim)
                            {
                                GamePiece diagPiece = gamePieces[diagX, diagY];
                                //为空则进行填充
                                if (diagPiece == null)
                                {
                                    aimPiece.MoveableComponent.move(diagX, diagY, dropSpeed);  //显示位置移动
                                    gamePieces[diagX, diagY] = aimPiece;    //坐标位置更新
                                    gamePieces[x, y] = null;    //坐标位置更新
                                    isRefresh = true;
                                   // return isRefresh;
                                    break;  //填充成功不再尝试其他方向
                                }

                            }
                        }
                    }
                }
            }
        return isRefresh;
    }
    //生成新的piece
    GamePiece SpawnNewPiece(int _x, int _y, PieceType _type)
    {         
        if( !piecePrefabDic.ContainsKey(_type) )
        {
            Debug.Log(_type.ToString() + " type no found");
            return null;
        }
        //生成鱼儿   
        GameObject newPieces = Instantiate(piecePrefabDic[_type], positionTrans(_x, _y, 0), Quaternion.identity) as GameObject;
        newPieces.transform.parent = this.transform;
        newPieces.name = _x + "," + _y;
        //获得组件
        gamePieces[_x, _y] = newPieces.GetComponent<GamePiece>();
        gamePieces[_x, _y].Init(_x, _y, this, _type);

        return gamePieces[_x, _y];
    }
    //生成初始化图上的固定位置
    void SpawnInitialPiece()
    {
        for (int i = 0; i < initialPieces.Length; i++)
        {
            int x = initialPieces[i].x;
            int y = initialPieces[i].y;
            //如果不出界则生成
            if(x>=0 && x<xdim && y>=0 && y<ydim)
                SpawnNewPiece(initialPieces[i].x, initialPieces[i].y, initialPieces[i].type);

        }
    }
    //设置交换的AB
    public void SetPieceA(GamePiece A)
    {
        pieceA = A;
        //Debug.Log("setA" + pieceA.X + " " + pieceA.Y);
    }
    public void SetPieceB(GamePiece B)
    {
        pieceB = B;
        //Debug.Log("setB" + pieceB.X + " " + pieceB.Y);
    }
    //是否可以交换
    bool isChange()
    {
        if (pieceA != null && pieceB != null)//已赋值
            if(pieceA.Type != PieceType.BUBBLE && pieceB.Type != PieceType.BUBBLE)  //不是气泡
                if ((pieceA.X == pieceB.X && Mathf.Abs(pieceA.Y - pieceB.Y) == 1)
                    || (pieceA.Y == pieceB.Y && Mathf.Abs(pieceA.X - pieceB.X) == 1))//邻居
                        return true;
            
         return false;
    }
    //交换
    public void Change()
    {
        //游戏结束
        if (gameOver)
            return;

        if (isChange())
        {
            int ax = pieceA.X; int ay = pieceA.Y;
            int bx = pieceB.X; int by = pieceB.Y;
            //坐标交换
            gamePieces[ax, ay] = pieceB;
            //gamePieces[ax, ay].X = ax;
            //gamePieces[ax, ay].Y = ay;
            gamePieces[bx, by] = pieceA;
            //gamePieces[bx, by].X = bx;
            //gamePieces[bx, by].Y = by;

            if (GetMatch(pieceA, bx, by) != null || GetMatch(pieceB, ax, ay) != null
                || pieceA.Type == PieceType.COLOR_CLEAR
                || pieceB.Type == PieceType.COLOR_CLEAR)//有连子或者彩虹颜色
            {
                //位置交换
                pieceA.MoveableComponent.move(bx, by);
                pieceB.MoveableComponent.move(ax, ay);
                //彩色交换，需要消除所有同颜色
                if(pieceA.Type == PieceType.COLOR_CLEAR && pieceB.isColor)
                {
                    ClearColorPiece colorPiece = pieceA.GetComponent<ClearColorPiece>();
                    if (colorPiece)
                        colorPiece.Color = pieceB.ColorComponent.ColorTypeRef;
                    ClearPiece(pieceA.X, pieceA.Y);
                }else if (pieceB.Type == PieceType.COLOR_CLEAR && pieceA.isColor)
                {
                    ClearColorPiece colorPiece = pieceB.GetComponent<ClearColorPiece>();
                    if (colorPiece)
                        colorPiece.Color = pieceA.ColorComponent.ColorTypeRef;
                    ClearPiece(pieceB.X, pieceB.Y);
                }
                //清除
                ClearAllMatch();
                //清除完毕后通知
                level.OnMove();
                //填充
                StartCoroutine(Fill());
            }
            else
            {
                //交换不成功，坐标换回来
                gamePieces[ax, ay] = pieceA;
                gamePieces[bx, by] = pieceB;
            }
        }
        //清除
        pieceA = null;
        pieceB = null;
    }
    //检测交换后是否有3个连子确定是否能交换
    List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
    {
        ColorPiece.ColorType color = piece.ColorComponent.ColorTypeRef; //颜色
        List<GamePiece> horizontalPiece = new List<GamePiece>();    //保存临时竖直方向的匹配
        List<GamePiece> verticalPiece = new List<GamePiece>();  //保存横向临时的匹配
        List<GamePiece> matchPieces = new List<GamePiece>(); //保存最终匹配结果

        //横向检测
        horizontalPiece.Add(piece);
        //先检测左边，再检测右边
        for(int dir = -1; dir<=1; dir += 2)
        {
            //邻近是否与自身连子
            for(int index = 1; index <= xdim; index++)
            {
                int x = newX + index * dir;
                if (x < 0 || x >= xdim)  //防止越界
                    break;
                //Debug.Log("横向:" + x + " " + newY);
                if (gamePieces[x, newY] != null && gamePieces[x, newY].isColor)
                    if (gamePieces[x, newY].ColorComponent.ColorTypeRef == color)
                    {
                        horizontalPiece.Add(gamePieces[x, newY]);
                        continue;//发现匹配可以继续
                    }

                break;//无匹配结束
            }
        }
        //有连子，检测T,L形
        if(horizontalPiece.Count >= 3)
        {
            //以每一个横向为基地查纵向是否有两个以上构成T||L
            for(int i=0; i<horizontalPiece.Count; i++)
            {
                matchPieces.Add(horizontalPiece[i]);    //先加入最终匹配结果
                for(int dir=-1; dir<=1; dir+=2)//-1向下，1向上
                {
                    for(int index = 1; index <= ydim; index++)
                    {
                        int y = horizontalPiece[i].Y + index * dir;
                        if (y < 0 || y >= ydim) //出界
                            break;
                        if (gamePieces[horizontalPiece[i].X, y] != null && gamePieces[horizontalPiece[i].X, y].isColor)
                            if (gamePieces[horizontalPiece[i].X, y].ColorComponent.ColorTypeRef == color)
                            {
                                verticalPiece.Add(gamePieces[horizontalPiece[i].X, y]);
                                continue;//发现匹配可以继续
                            }

                        break;//无匹配结束
                    }
                }
                //如果大于2，说明有T,L出现
                if(verticalPiece.Count >= 2)
                    for(int k=0; k < verticalPiece.Count; k++)//, Debug.Log("横向有L"))
                        matchPieces.Add(verticalPiece[k]);
                //出现可能性？
                //110
                //110
                //111
                verticalPiece.Clear();  //清空计算下一个横向piece

            }
            //ShowPiecesAxis(matchPieces);
            return matchPieces;    //大于三返回真
        }


        //横向没有，进行纵向检测
        horizontalPiece.Clear();
        verticalPiece.Clear();
        matchPieces.Clear();//清空

        verticalPiece.Add(piece);
        //先检测下边，再检测上边
        for (int dir = -1; dir <= 1; dir += 2)
        {
            //邻近两个是否与自身连子
            for (int index = 1; index <= ydim; index++)
            {
                int y = newY + index * dir;
                if (y < 0 || y >= ydim)  //防止越界
                    break;
                //Debug.Log("纵向:" + newX + " " + y);
                if(gamePieces[newX, y] != null && gamePieces[newX, y].isColor)
                    if (gamePieces[newX, y].ColorComponent.ColorTypeRef == color)
                    {
                        verticalPiece.Add(gamePieces[newX, y]);
                        continue;//发现匹配可以继续
                    }

                break;//无匹配结束
            }
        }
        //纵向有三连子,检测L,T
        if (verticalPiece.Count >= 3)
        {
            //以每一个纵向为基查横向是否有两个以上构成T||L
            for (int i = 0; i < verticalPiece.Count; i++)
            {
                matchPieces.Add(verticalPiece[i]);    //先加入最终匹配结果
                for (int dir = -1; dir <= 1; dir += 2)//-1向下，1向上
                {
                    for (int index = 1; index <= xdim; index++)
                    {
                        int x = verticalPiece[i].X + index * dir;
                        if (x < 0 || x >= ydim) //出界
                            break;
                        if (gamePieces[x, verticalPiece[i].Y] != null && gamePieces[x, verticalPiece[i].Y].isColor)
                            if (gamePieces[x, verticalPiece[i].Y].ColorComponent.ColorTypeRef == color)
                            {
                                horizontalPiece.Add(gamePieces[x, verticalPiece[i].Y]);
                                continue;//发现匹配继续
                            }

                        break;//无匹配
                    }
                }
                //如果大于2，说明有T,L出现
                if (horizontalPiece.Count >= 2)
                    for (int k = 0; k < horizontalPiece.Count; k++)//, Debug.Log("纵向有L"))
                        matchPieces.Add(horizontalPiece[k]);
                horizontalPiece.Clear();  //清空计算下一个纵向piece
            }
            //ShowPiecesAxis(matchPieces);
            return matchPieces;    //大于三返回真
        }      
        else
            return null;   //横向纵向都没有
    }
    //清除全局
    bool ClearAllMatch()
    {
        bool needRefill = false;
        //遍历每一个Piece
        for(int i=0; i<xdim; i++)
        {
            for(int j=0; j<ydim; j++)
            {
                if (gamePieces[i, j] != null && gamePieces[i, j].Type == PieceType.NORMOL &&  
                    gamePieces[i, j].isClear && !gamePieces[i,j].ClearComponent.isBeingClear)
                {
                    //以各个piece为基点查找匹配
                    List<GamePiece> matchPieces = GetMatch(gamePieces[i, j], gamePieces[i, j].X, gamePieces[i, j].Y);
                    if (matchPieces != null)    //有配对
                    {
                        //特殊元素默认为系统出现的坐标
                        int middle = matchPieces.Count / 2;
                        int specialPieceX = matchPieces[middle].X;
                        int specialPieceY = matchPieces[middle].Y;
                        ColorPiece.ColorType specialPieceColor = matchPieces[middle].ColorComponent.ColorTypeRef;
                        PieceType specialPieceType = (PieceType)Random.Range((int)PieceType.ROW_CLEAR, (int)PieceType.COLUMN_CLEAR);

                        bool isNeedSpecial = false;//是否需要生成特殊元素
                        if (matchPieces.Count == 4)
                        {
                           // Debug.Log("match 4");
                            isNeedSpecial = true;
                            //用户交换出现的T|L
                            if (pieceA != null && pieceB != null)
                            {
                                //Debug.Log("user cause");
                                //横纵确认
                                if (pieceA.X == pieceB.X)
                                    specialPieceType = PieceType.ROW_CLEAR;
                                else
                                    specialPieceType = PieceType.COLUMN_CLEAR;
                                //坐标确认
                                if (pieceA.Type == matchPieces[0].Type)
                                {
                                    specialPieceX = pieceA.X;
                                    specialPieceY = pieceA.Y;
                                }
                                else
                                {
                                    specialPieceX = pieceB.X;
                                    specialPieceY = pieceB.Y;
                                }
                            }
                        }
                        else if(matchPieces.Count >= 5)
                        {
                            //Debug.Log("match 5");
                            isNeedSpecial = true;
                            //用户交换出现的大于5
                            if (pieceA != null && pieceB != null)
                            {
                                //Debug.Log("user cause");
                                specialPieceType = PieceType.COLOR_CLEAR;
                                specialPieceColor = ColorPiece.ColorType.ANY;   //彩色
                                //坐标确认
                                if (pieceA.Type == matchPieces[0].Type)
                                {
                                    specialPieceX = pieceA.X;
                                    specialPieceY = pieceA.Y;
                                }
                                else
                                {
                                    specialPieceX = pieceB.X;
                                    specialPieceY = pieceB.Y;
                                }
                            }
                        }
                        //清除
                        ClearPieces(matchPieces);
                        
                        //生成特殊元素
                        if(isNeedSpecial)
                        {
                            gamePieces[specialPieceX, specialPieceY] = SpawnNewPiece(specialPieceX, specialPieceY, specialPieceType);
                            gamePieces[specialPieceX, specialPieceY].ColorComponent.setColor(specialPieceColor);
                        }
                        
                        needRefill = true;  //需要重新填充
                    }     
                }
            }
        }
        return needRefill;
    }
    //清除单个
    void ClearPiece(int x, int y)
    {
        if (gamePieces[x, y] == null || !gamePieces[x, y].isClear)
            return;

        GamePiece tempPiece = gamePieces[x,y];
        gamePieces[x,y] = null;    //以便在消除动画播放中上方也能下落到此
        //Debug.Log("ready to clear:" + x + " " + y);
        if (tempPiece.Type != PieceType.BUBBLE)
            ClearBubble(tempPiece); //清除气泡
        tempPiece.ClearComponent.Clear();
    }
    //清除piece组
    void ClearPieces(List<GamePiece> pieces)
    {
        for (int i = 0; i < pieces.Count; i++)
            if (pieces[i].isClear)
                ClearPiece(pieces[i].X, pieces[i].Y);
    }
    //清除一整行
    public void ClearRow(int row)
    {
        for (int i = 0; i < xdim; i++)
            if (gamePieces[i, row] != null && gamePieces[i, row].isClear)
                ClearPiece(i, row);
    }
    //清除一整列
    public void ClearColumn(int column)
    {
        for (int i = 0; i < ydim; i++)
            if (gamePieces[column, i] != null && gamePieces[column, i].isClear)
                ClearPiece(column, i);
    }
    //清除所有同色
    public void ClearColor(ColorPiece.ColorType color)
    {
        for(int x=0; x<xdim; x++)
            for(int y=0; y<ydim; y++)
                if(gamePieces[x,y] != null && gamePieces[x,y].isColor 
                    && gamePieces[x,y].isClear )
                    if( gamePieces[x,y].ColorComponent.ColorTypeRef == color
                        || color == ColorPiece.ColorType.ANY)//相同颜色，或者交换两个彩虹清除所有颜色
                            ClearPiece(x, y);
    }
    //清除气泡
    void ClearBubble(GamePiece aimPiece)
    {
        //上下左右十字架
        int x = aimPiece.X;
        int y = aimPiece.Y;
        if (x - 1 >= 0 && gamePieces[x - 1, y] != null && gamePieces[x - 1, y].Type == PieceType.BUBBLE
             && gamePieces[x - 1, y].isClear)
        {
            gamePieces[x - 1, y].ClearComponent.Clear();
            gamePieces[x - 1, y] = null;
        }

        if (x + 1 < xdim && gamePieces[x + 1, y] != null && gamePieces[x + 1, y].Type == PieceType.BUBBLE
             && gamePieces[x + 1, y].isClear)
        {
            gamePieces[x + 1, y].ClearComponent.Clear();
            gamePieces[x + 1, y] = null;
        }

        if (y - 1 >= 0 && gamePieces[x, y - 1] != null && gamePieces[x, y - 1].Type == PieceType.BUBBLE
                     && gamePieces[x, y - 1].isClear)
        {
            gamePieces[x, y - 1].ClearComponent.Clear();
            gamePieces[x, y - 1] = null;
        }

        if (y + 1 < ydim && gamePieces[x, y + 1] != null && gamePieces[x, y + 1].Type == PieceType.BUBBLE
                     && gamePieces[x, y + 1].isClear)
        {
            gamePieces[x, y + 1].ClearComponent.Clear();
            gamePieces[x, y + 1] = null;
        }

    }
    //游戏结束
    public void GameOver()
    {
        gameOver = true;
    }
    //debug显示表
    void ShowPiecesAxis(List<GamePiece> pieces)
    {
        for (int i = 0; i < pieces.Count; i++)
            Debug.Log(pieces[i].X + " " + pieces[i].Y);
    }
}
