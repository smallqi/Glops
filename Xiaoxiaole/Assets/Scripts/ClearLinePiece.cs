using UnityEngine;
using System.Collections;

public class ClearLinePiece : ClearablePiece {

    public bool isRow;  //判断是横纵

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void Clear()
    {
        base.Clear();
        if (isRow)
            piece.GridRef.ClearRow(piece.Y);
        else
            piece.GridRef.ClearColumn(piece.X);
    }
}
