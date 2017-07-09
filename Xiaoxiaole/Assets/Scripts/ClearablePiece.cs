using UnityEngine;
using System.Collections;

public class ClearablePiece : MonoBehaviour {

    public AnimationClip clearAnimation;    //清除动画

    public bool isBeingClear = false;
    protected GamePiece piece;

    private void Awake()
    {
        piece = GetComponent<GamePiece>();
    }
    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public virtual void Clear()
    {
        isBeingClear = true;
        piece.GridRef.level.OnPieceCleared(piece);  //通知消除
        StartCoroutine(ClearCoroutine());
    }
    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();
        if(animator != null)    //播放消除动画
        {
            animator.Play(clearAnimation.name);
        }
        yield return new WaitForSeconds(clearAnimation.length); //等待动画播完
        Destroy(gameObject);
    }
}
