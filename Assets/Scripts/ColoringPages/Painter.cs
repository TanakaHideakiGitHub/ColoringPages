using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Tanaka;

public class Painter : MonoBehaviour
{
    enum ColorType
    {
        Erazer = 0,
        Red,
        Green,
        Blue,
        White,
        Gray
    }

    /// <summary>
    /// 輪郭表示用テクスチャ
    /// </summary>
    [SerializeField]
    private EdgeTexture edgeTex;

    /// <summary>
    /// 書き込み用
    /// </summary>
    [SerializeField]
    private RawImage writeRawImage;

    /// <summary>
    /// スクショ保存
    /// </summary>
    private SaveColoringPage saveScreen;

    public GameObject Menus;

    public ImageScrollController ImageScroller;

    private Page writePage;

    /// <summary>
    /// 書き込み不可状態かどうか
    /// </summary>
    public bool IsUnpaintable { get; set; }

    void Start ()
    {
        saveScreen = new SaveColoringPage();

        CreatePage();
        ImageScroller.Initialize();
        ImageScroller.gameObject.SetActive(false);
    }
	
	void Update ()
    {
        if (IsUnpaintable)
            return;
        // マウスクリック
        if (UtilTouch.GetTouch() != TouchInfo.None)
            writePage.UpdatePixel(UtilTouch.GetTouchPosition());
        else
            writePage.ResetPrePosition();
    }

    /// <summary>
    /// 書き込み用テクスチャの再生成
    /// </summary>
    public void CreatePage()
    {
        writePage = new Page();
        writePage.SetWriteTexture(writeRawImage);
    }

    /// <summary>
    /// 保存済み画像をスクロールで表示
    /// </summary>
    public void OpenImageScroller()
    {
        if (!ImageScroller.IsOpend)
            ImageScroller.Open();
        else
            ImageScroller.Close();
    }

    public void SetEdgeTexture(Texture2D tex)
    {
        edgeTex.SetTextureByAspect(tex);
    }

    public void ChangeScale(int addScl)
    {
        writePage.ChangeScale(addScl);
    }
    public void ColorChange(Color col)
    {
        writePage.ColorChange(col);
    }

    public void OnSaveButton()
    {
        StartCoroutine(SaveScreenShot());
    }

    private IEnumerator SaveScreenShot()
    {
        Menus.SetActive(false);

        yield return StartCoroutine(saveScreen.Save(edgeTex));

        Menus.SetActive(true);
    }

}
