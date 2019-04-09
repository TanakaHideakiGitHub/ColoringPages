using Tanaka;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveColoringPage
{
    public IEnumerator Save(EdgeTexture EdgeTex)
    {
        yield return new WaitForEndOfFrame();

        var texRate = (float)EdgeTex.Texture.width / EdgeTex.Texture.height;
        // 画面全体ではなく表示されている書き込み用キャンバスと同サイズにする
        var height = Screen.height;
        var width = Mathf.RoundToInt(Screen.height * texRate * EdgeTexture.RESOLUTION_RATE);

        var canvasRate = 1820f / 1920f;
        var left = (Screen.width * canvasRate - width) * 0.5f;

        // 横長の画像の場合
        var bottom = 0f;
        if (width > Screen.width)
        {
            // 画面幅以上のwidthを画面幅に合わせる
            var applyRate = Screen.width * canvasRate / width;
            width =  Mathf.RoundToInt(applyRate * width);
            height = Mathf.RoundToInt(applyRate * height);
            left = 0;
            bottom = EdgeTex.RectTransformBottom * 0.5f;
        }

        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.name = EdgeTex.Texture.name;
        tex.ReadPixels(new Rect(left, bottom, width, height), 0, 0);
        tex.Apply();

        TextureScale.Point(tex, Mathf.RoundToInt(Screen.height * texRate), height);
        yield return EdgeTex.StartCoroutine(WriteFile(tex));
    }

    private IEnumerator WriteFile(Texture2D tex)
    {
        var fileName = "Screenshot" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";

        var path = Utils.GetWriteFolderPath(tex.name);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        path += fileName;

        var bytes = tex.EncodeToPNG();
#if !UNITY_EDITOR
        //var bytes = tex.GetRawTextureData();
        File.WriteAllBytes(path, bytes);
        yield return new WaitForEndOfFrame();

        while (!File.Exists(path))
            yield return new WaitForEndOfFrame();

        UtilsAndroid.ScanFile(path, null);
#else

        Debug.Log("WriteFile");
        File.WriteAllBytes(path, bytes);
        yield return null;
#endif
    }
}
