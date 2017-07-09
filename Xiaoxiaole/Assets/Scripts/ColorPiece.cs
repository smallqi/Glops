using UnityEngine;
using System.Collections.Generic;

public class ColorPiece: MonoBehaviour {

    //所有颜色的种类
    public enum ColorType
    {
        YELLOW,
        PURPLE,
        RED,
        BLUE,
        GREEN,
        PINK,
        ANY,
        COUNT
    };
    
    //颜色与精灵对应
    [System.Serializable]
    public struct ColorSprite
    {
        public ColorType type;
        public Sprite sprite;
    }
    public ColorSprite[] colorSprite;
    public int ColorNumber
    {
        get { return colorSprite.Length; }
    }
    //索引保存
    private Dictionary<ColorType, Sprite> colorSpriteDic;

    //自身的颜色
    private ColorType colorType;
    public ColorType ColorTypeRef
    {
        get { return colorType; }
        set { setColor(value); }
    }
    //自身的精灵
    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Awake () {
        //获得组件
        spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.Log("no find renderer");
        //加入索引
        colorSpriteDic = new Dictionary<ColorType, Sprite>();
        for (int i = 0; i < colorSprite.Length; i++)
            if (!colorSpriteDic.ContainsKey(colorSprite[i].type))
            {
                //Debug.Log(colorSprite[i].type.ToString());
                colorSpriteDic.Add(colorSprite[i].type, colorSprite[i].sprite);
            }
    }
                
	
	// Update is called once per frame
	void Update () {
	
	}
    //更改颜色和显示的精灵
    public void setColor(ColorType _type)
    {
        colorType = _type;
        if (colorSpriteDic.ContainsKey(_type))
            spriteRenderer.sprite = colorSpriteDic[_type];
    }
}
