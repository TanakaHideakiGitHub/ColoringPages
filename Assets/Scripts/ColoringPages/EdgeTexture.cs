using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class EdgeTexture : MonoBehaviour
{
    /// <summary>
    /// ベース解像度と実際の解像度の比率
    /// </summary>
    public static readonly float RESOLUTION_RATE = (9f / 16f) / ((float)Screen.height / Screen.width);
    /// <summary>
    /// 16:9の場合の塗り絵用キャンバスX幅
    /// </summary>
    private static readonly float MAX_EDGE_TEX_X = 1820f;
    /// <summary>
    /// 16:9の場合の塗り絵用キャンバスY幅
    /// </summary>
    private static readonly float MAX_EDGE_TEX_Y = 1080f;

    /// <summary>
    /// 輪郭表示用オブジェクト
    /// </summary>
    private RawImage edgeTex;
    public Texture Texture { get { return edgeTex.texture; } set { edgeTex.texture = value; } }
    public RectTransform RectTransform { get { return edgeTex.rectTransform; } }

    public float RectTransformLeft { get; private set; }
    public float RectTransformBottom { get; private set; }

    void Awake ()
    {
        edgeTex = GetComponent<RawImage>();
    }
	
//    public void LoadTexture()
//    {
//#if !UNITY_EDITOR
//        var accessor = new AndroidPluginAccessor();
//        accessor.CallStatic(AndroidPluginAccessor.OPEN_CAM_ROLL);
//#else
//        var tex = Resources.Load<Texture2D>("Textures/ColoringPages/an13");
//        SetTextureByAspect(tex);
//#endif
//    }

    public void SetTextureByAspect(Texture2D tex)
    {
        var rate = 0f;
        var w = 0f;
        var h = 0f;
        var edgeTexRatio = (float)tex.width / tex.height;
        var writePageRatio = Page.WRITE_ASPECT_RATIO_X;
        if (tex.width < MAX_EDGE_TEX_X && edgeTexRatio < writePageRatio)
        {
            rate = MAX_EDGE_TEX_Y / tex.height;
            w = MAX_EDGE_TEX_X - tex.width * rate;
            w *= RESOLUTION_RATE;
        }
        else
        {
            rate = MAX_EDGE_TEX_X / tex.width;
            h = MAX_EDGE_TEX_Y - tex.height * rate;
            // 画面幅よりでかくなるようなら再調整
            if(h < 0)
            {
                h = 0;
                rate = MAX_EDGE_TEX_Y / tex.height;
                w = MAX_EDGE_TEX_X - tex.width * rate;
            }
        }

        var left = w * 0.5f;
        var right = -w * 0.5f;
        var bottom = h * 0.5f;
        var top = -h * 0.5f;
        edgeTex.rectTransform.offsetMax = new Vector2(right, top);
        edgeTex.rectTransform.offsetMin = new Vector2(left, bottom);
        edgeTex.texture = tex;

        RectTransformLeft = left;
        RectTransformBottom = bottom;
    }
}
