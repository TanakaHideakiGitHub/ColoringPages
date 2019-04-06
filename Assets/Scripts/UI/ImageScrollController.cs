using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImageScrollController : MonoBehaviour
{
    /// <summary>
    /// 表示に利用するオブジェクト数
    /// </summary>
    private static readonly int MAX_VIEW_ITEMS = 12;

    [SerializeField]
    private ScrollViewer viewer;

    [SerializeField]
    private Painter painter;

    private ImageScrollModel imagesModel;

    /// <summary>
    /// スクロールが開いているかどうか
    /// </summary>
    public bool IsOpend { get { return viewer.gameObject.activeSelf; } }

    private void Start()
    {
        viewer.SetNodeButtonAction(SetNodeTexture4EdgeTexture);
        Close();
    }

    public void Initialize()
    {
        var items = viewer.ViewContent.GetComponentsInChildren<CanvasRenderer>().Select(i => i.GetComponent<RectTransform>()).ToArray();
        imagesModel = new ImageScrollModel(items, MAX_VIEW_ITEMS);

        viewer.Initialize(MAX_VIEW_ITEMS, imagesModel.ItemsCount, imagesModel.SpriteList);
        viewer.OnUpdateItemsByScroll = OnUpdateItemsByScroll;

        // スクロールによる画像更新処理を登録
        imagesModel.OnUpdateImage = viewer.OnUpdateImage;
    }
    public void Open()
    {
        painter.IsUnpaintable = true;
        viewer.ResetScrollerPosition();
        viewer.gameObject.SetActive(true);
    }
    public void Close()
    {
        viewer.gameObject.SetActive(false);
        StartCoroutine(SetCloseAfterFrame());
    }
    private IEnumerator SetCloseAfterFrame()
    {
        // スクロールを閉じると同時にフラグOFFだと閉じるためのクリックで
        // 書き込みが行われてしまうので1F待つ
        yield return null;
        painter.IsUnpaintable = false;
    }

    /// <summary>
    /// スクロールで選択されたテクスチャを表示用オブジェクトにセット
    /// </summary>
    /// <param name="tex"></param>
    private void SetNodeTexture4EdgeTexture(Texture2D tex)
    {
        painter.SetEdgeTexture(tex);
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
