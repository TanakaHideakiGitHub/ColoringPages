using Tanaka;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PictureBookAnimation : MonoBehaviour
{
    private static readonly string PATH = "Textures/ColoringPages/Preset/";

    /// <summary>
    /// タッチアニメーションさせるかどうか
    /// </summary>
    public bool IsPlayableTouchedAnime = false;
    /// <summary>
    /// タッチアニメーションカーブ
    /// </summary>
    public AnimationCurve TouchedCurve;

    /// <summary>
    /// タッチアニメ再生中
    /// </summary>
    private bool isPlayingTouched;
    /// <summary>
    /// タッチアニメの長さ(秒)
    /// </summary>
    private float touchedAnimeTime = 1f;
    /// <summary>
    /// タッチアニメの経過時間
    /// </summary>
    private float touchedAnimeElapsed = 0f;
    /// <summary>
    /// 初期スケール
    /// </summary>
    private Vector3 baseScale;

    void Start ()
    {
        var kyes = TouchedCurve.keys;
        touchedAnimeTime = kyes[TouchedCurve.length - 1].time;
        baseScale = transform.localScale;
        SetRandomTexture();
    }
	
	void Update ()
    {
        OnTouchedAnimation();
    }

    public void OnTouched()
    {
        isPlayingTouched = true;
        ResetBeforeTouched();
    }

    /// <summary>
    /// タッチアニメ開始前の状態にリセット
    /// </summary>
    private void ResetBeforeTouched()
    {
        touchedAnimeElapsed = 0;
        transform.localScale = baseScale;
    }

    /// <summary>
    /// タッチ時のアニメーション
    /// </summary>
    private void OnTouchedAnimation()
    {
        if (!IsPlayableTouchedAnime || !isPlayingTouched)
            return;

        if (touchedAnimeElapsed < touchedAnimeTime)
        {
            var val = TouchedCurve.Evaluate(touchedAnimeElapsed);
            transform.localScale = baseScale * val;
            touchedAnimeElapsed += Time.deltaTime;
        }
        else
        {
            isPlayingTouched = false;
            ResetBeforeTouched();
        }
    }

    /// <summary>
    /// ランダムにお絵描きした画像をロード
    /// </summary>
    private void SetRandomTexture()
    {
        //オブジェクトの名前からフォルダパスを取得
        var path = Utils.GetWriteFolderPath(name);

        if (!Directory.Exists(path))
            return;

        // ランダムに選択
        var filePaths = Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly).OrderBy(f => File.GetCreationTime(f)).ToArray();
        if (filePaths.Length == 0)
            return;

        int num = Random.Range(0, filePaths.Length);
        TextureLoad(filePaths[num]);
    }

    /// <summary>
    /// お絵描きした画像をロード
    /// </summary>
    private void TextureLoad(string filePath)
    {
        // サイズ取得用に元画像をロード
        var tmp = Resources.Load<Texture2D>(PATH + name);
        var tex = Utils.LoadTextureByFileIO(filePath, tmp.width, tmp.height);
        //var tex = Utils.LoadTextureByFileIO(filePath, tmp.width, tmp.height);

        SetTextureOnShader(tex);
    }

    /// <summary>
    /// シェーダーにテクスチャを渡す
    /// </summary>
    /// <param name="tex"></param>
    private void SetTextureOnShader(Texture2D tex)
    {
        var mat = GetComponent<SpriteRenderer>().material;
        mat.SetTexture("_SourceTex", tex);
    }
}
