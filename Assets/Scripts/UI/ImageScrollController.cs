using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImageScrollController : SingletonMonoBehaviour<ImageScrollController>
{
    /// <summary>
    /// 表示に利用するオブジェクト数
    /// </summary>
    private static readonly int MAX_VIEW_ITEMS = 12;

    [SerializeField]
    private ScrollViewer viewer;

    /// <summary>
    /// ノードが押下されたときに処理する関数の登録
    /// </summary>
    private Action<Texture2D> onNodePush;

    private ImageScrollModel imagesModel;

    /// <summary>
    /// スクロールが開いているかどうか
    /// </summary>
    public bool IsOpend { get { return viewer.gameObject.activeSelf; } }

    void Start()
    {
        Close();
    }

    public void Initialize(string folderPath)
    {
        var items = viewer.ViewContent.GetComponentsInChildren<CanvasRenderer>().Select(i => i.GetComponent<RectTransform>()).ToArray();
        imagesModel = new ImageScrollModel(items, MAX_VIEW_ITEMS);
        imagesModel.Initialize(folderPath);
        viewer.Initialize(MAX_VIEW_ITEMS, imagesModel.ItemsCount, imagesModel.SpriteList);
        viewer.OnUpdateItemsByScroll = OnUpdateItemsByScroll;

        // スクロールによる画像更新処理を登録
        imagesModel.OnUpdateImage = viewer.OnUpdateImage;
    }
    public void Open(string folderPath, Action<Texture2D> onNodePush)
    {
        Initialize(folderPath);
        this.onNodePush = onNodePush;
        viewer.SetNodeButtonAction(OnNodePush);
        viewer.ResetScrollerPosition();
        viewer.gameObject.SetActive(true);
    }
    public void Close()
    {
        viewer.gameObject.SetActive(false);
    }

    /// <summary>
    /// ノード押下されたときの処理
    /// </summary>
    /// <param name="tex"></param>
    private void OnNodePush(Texture2D tex)
    {
        if(onNodePush != null)
            onNodePush(tex);
        Close();
    }

    /// <summary>
    /// スクロールによって画像更新する際の処理の橋渡し
    /// </summary>
    /// <param name="columnCnt"></param>
    /// <param name="currentRow"></param>
    /// <param name="isScrollDown"></param>
    /// <param name="list"></param>
    private void OnUpdateItemsByScroll(int columnCnt, int currentRow, bool isScrollDown, LinkedList<RectTransform> list)
    {
        imagesModel.OnChangeDrawByScroll(columnCnt, currentRow, isScrollDown, list);
        Resources.UnloadUnusedAssets();
    }
}
