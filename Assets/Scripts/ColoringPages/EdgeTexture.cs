using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class EdgeTexture : MonoBehaviour
{
    public static float WIDTH = 0;
    public static float HEIGHT = 0;
    /// <summary>
    /// ベース解像度と実際の解像度の比率
    /// </summary>
    public static readonly float RESOLUTION_RATE = (9f / 16f) / ((float)Screen.height / Screen.width);
    /// <summary>
    /// 16:9の場合の塗り絵用キャンバスX幅
    /// </summary>
    private static readonly float MAX_EDGE_TEX_X = 1820;
    /// <summary>
    /// 16:9の場合の塗り絵用キャンバスY幅
    /// </summary>
    private static readonly float MAX_EDGE_TEX_Y = 1080;

    private RawImage edgeTex;
    public Texture Texture { get { return edgeTex.texture; } set { edgeTex.texture = value; } }
    public RectTransform RectTransform { get { return edgeTex.rectTransform; } }

    void Awake ()
    {
        edgeTex = GetComponent<RawImage>();
    }
	
    public void LoadTexture()
    {
#if !UNITY_EDITOR
        var accessor = new AndroidPluginAccessor();
        accessor.CallStatic(AndroidPluginAccessor.OPEN_CAM_ROLL);
#else
        //var t = Resources.Load<Texture2D>("Textures/1yamv0g1");
        //// XYでテクスチャの端点を決められ、WHでテクスチャサイズを変更できる
        ////EdgeTex.uvRect = new Rect(EdgeTex.uvRect.x, EdgeTex.uvRect.y, EdgeTex.texture.width, EdgeTex.texture.height);
        ////EdgeTex.SetNativeSize();
        //byte[] bytReadBinary = File.ReadAllBytes("E:/Program/TestProject/Assets/Resources/Textures/1yamv0g1");
        //var width = Screen.width;
        //var height = Screen.height;
        //var tex = new Texture2D(width, height);
        //tex.LoadImage(bytReadBinary);
        //tex.filterMode = FilterMode.Trilinear;
        //tex.Apply();
        //var tex = Tanaka.BitmapLoader.Load("E:/Program/TestProject/Assets/Resources/Textures/c0kqmtaw.bmp");
        var tex = Resources.Load<Texture2D>("Textures/ColoringPages/an13");
        SetTextureByAspect(tex);
#endif
    }

    public void SetTextureByAspect(Texture2D tex)
    {
        var rate = 0f;
        var w = 0f;
        var h = 0f;
        var r1 = (float)tex.width / tex.height;
        var r2 = Page.WRITE_ASPECT_RATIO_X;
        if (tex.width < MAX_EDGE_TEX_X && r1 < r2/*tex.width <= tex.height*/)
        {
            rate = MAX_EDGE_TEX_Y / tex.height;
            w = MAX_EDGE_TEX_X - tex.width * rate;
            w *= RESOLUTION_RATE;
        }
        else
        {
            rate = MAX_EDGE_TEX_X / tex.width;
            h = MAX_EDGE_TEX_Y - tex.height * rate;
            if(h < 0)
            {
                h = 0;
                rate = MAX_EDGE_TEX_Y / tex.height;
                w = MAX_EDGE_TEX_X - tex.width * rate;
            }
        }
        Debug.Log(rate);

        var left = w * 0.5f;
        var right = -w * 0.5f;
        var bottom = h * 0.5f;
        var top = -h * 0.5f;
        edgeTex.rectTransform.offsetMax = new Vector2(right, top);
        edgeTex.rectTransform.offsetMin = new Vector2(left, bottom);
        edgeTex.texture = tex;

        WIDTH = left;
        HEIGHT = bottom;
    }
}
