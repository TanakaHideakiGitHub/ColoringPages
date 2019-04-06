using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Page
{
    /// <summary>
    /// 横1920のとき、書き込みテクスチャのサイズは1820にするので
    /// その割合を出しておく
    /// </summary>
    private static readonly float WRITE_ASPECT_RATIO_X = 1820f / 1920f;

    /// <summary>
    /// 書き込み用テクスチャ
    /// </summary>
    private Texture2D whiteCanvas;

    /// <summary>
    /// 前Fでタップしていた座標保持用
    /// </summary>
    private Vector2 prePos;

    /// <summary>
    /// 書き込み用ブラシ
    /// </summary>
    private class Brush
    {
        public static readonly int SCALING_SIZE = 5;
        public int Width = 8;
        public int Height = 8;
        /// <summary> 現在色保持用 </summary>
        public Color Color = Color.gray;
        /// <summary> ブラシサイズ分の色配列 </summary>
        public Color[] Colors;

        public Brush()
        {
            Colors = new Color[Width * Height];
            for (int i = 0; i < Colors.Length; ++i)
                Colors[i] = Color;
        }

        public void UpdateColor(Color col)
        {
            Color = col;
            for (int i = 0; i < Colors.Length; ++i)
                Colors[i] = Color;
        }
    }
    private Brush brush;

    public int TexWidth  { get { return whiteCanvas.width; } }
    public int TexHeight { get { return whiteCanvas.height; } }

    public Page()
    {
        whiteCanvas = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        brush = new Brush();
        prePos = Vector2.zero;
        Clear();
    }

    public void ResetPrePosition()
    {
        prePos = Vector2.zero;
    }

    public void ChangeScale(int addScl)
    {
        brush.Width = Mathf.Max(1, brush.Width + addScl * Brush.SCALING_SIZE);
    }
    public void ColorChange(Color col)
    {
        brush.UpdateColor(col);
    }

    /// <summary>
    /// ピクセル更新
    /// </summary>
    /// <param name="position"></param>
    public void UpdatePixel(Vector2 position)
    {
        var rx = position.x / (Screen.width * WRITE_ASPECT_RATIO_X) * TexWidth;
        var ry = position.y / Screen.height * TexHeight;

        if (rx < 0 || rx > TexWidth || ry < 0 || ry > TexHeight)
        {
            prePos = Vector2.zero;
            return;
        }

        if (prePos.sqrMagnitude > 0.5f)
        {
            var pos = new Vector2(rx, ry);
            var nor = (pos - prePos).normalized;
            int len = (int)Mathf.Abs(Vector2.Distance(pos, prePos));
            var dis = Vector2.zero;
            for (int i = 0; i < len; ++i)
            {
                dis += nor;
                WriteCircleNotApply((int)(rx + dis.x), (int)(ry + dis.y), brush.Color, brush.Width);
            }
        }

        WriteCircle((int)rx, (int)ry, brush.Color, brush.Width);
        prePos = new Vector2(rx, ry);
    }

    /// <summary>
    /// ピクセル単位での書き込み
    /// </summary>
    /// <param name="pixelX"></param>
    /// <param name="pixelY"></param>
    /// <param name="color"></param>
    public void WritePixel(int pixelX, int pixelY, Color color)
    {
        whiteCanvas.SetPixel(pixelX, pixelY, color);
        whiteCanvas.Apply();
    }
    /// <summary>
    /// 四角形での書き込み
    /// </summary>
    /// <param name="pixelX"></param>
    /// <param name="pixelY"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="colors"></param>
    public void WriteSquare(int pixelX, int pixelY, int width, int height, Color[] colors)
    {
        whiteCanvas.SetPixels(pixelX, pixelY, width, height, colors);
        whiteCanvas.Apply();
    }
    /// <summary>
    /// 円で書き込むがApplyはしない
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="color"></param>
    /// <param name="radius"></param>
    public void WriteCircleNotApply(int x, int y, Color color, int radius)
    {
        int r = radius;
        for (int iy = -r; iy < r; iy++)
        {
            for (int ix = -r; ix < r; ix++)
            {
                if (ix * ix + iy * iy < r * r)
                {
                    var tx = x + ix;
                    var ty = y + iy;
                    if (tx >= 0 && ty >= 0 && tx < TexWidth && ty < TexHeight)
                        whiteCanvas.SetPixel(x + ix, y + iy, color);
                }
            }
        }
    }
    /// <summary>
    /// 円での書き込み
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="color"></param>
    /// <param name="radius"></param>
    public void WriteCircle(int x, int y, Color color, int radius)
    {
        WriteCircleNotApply(x, y, color, radius);
        whiteCanvas.Apply();
    }
    public void Clear()
    {
        for (int x = 0; x < Screen.width; ++x)
        {
            for (int y = 0; y < Screen.height; ++y)
                whiteCanvas.SetPixel(x, y, Color.white);
        }
        whiteCanvas.Apply();
    }

    /// <summary>
    /// 指定したオブジェクトに書き込み用テクスチャを登録する
    /// </summary>
    /// <param name="rawImage"></param>
    public void SetWriteTexture(RawImage rawImage)
    {
        rawImage.texture = whiteCanvas;
    }
}
