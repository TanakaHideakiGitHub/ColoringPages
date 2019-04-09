using Tanaka;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuView : MonoBehaviour
{
    private ColorMenuButton[] ColorButtons;
    private ScaleMenuButton[] ScaleButtons;

    /// <summary>
    /// 固有機能を持ったメニューボタン
    /// </summary>
    [SerializeField]
    private UniqueMenuButton[] UniqueButtons;

    /// <summary>
    /// メインメニュー用ボタンの親キャンバス
    /// </summary>
    [SerializeField]
    private GameObject MainMenuButtonCanvas;

    /// <summary>
    /// 色変更などのパレット並べる用レイアウトグループ
    /// </summary>
    [SerializeField]
    private GameObject ColorPaletteParent;

    /// <summary>
    /// パレット移動用ボタンのテキスト
    /// </summary>
    [SerializeField]
    private Text PaletteMoveButtonText;

    private int ColorPaletteMoveInHash = Animator.StringToHash("UIColorPaletteAnimation");
    private int ColorPaletteMoveOutHash = Animator.StringToHash("ReturnUIColorPaletteAnimation");

    /// <summary>
    /// パレット移動中かどうか
    /// </summary>
    private bool isMovingPalette = false;

    /// <summary>
    /// パレット開いた状態かどうか
    /// </summary>
    public bool IsOpendColorPalette { get; private set; }


    public Animator ColorPaletteAnime;

    void Start ()
    {
        ColorButtons = GetComponentsInChildren<ColorMenuButton>();
        ScaleButtons = GetComponentsInChildren<ScaleMenuButton>();
        UniqueButtons = GetComponentsInChildren<UniqueMenuButton>();
    }

    public void SetColorButtonListener (Action<Color> OnPush)
    {
        foreach (var cb in ColorButtons)
            cb.OnPush = OnPush;
    }
    public void SetScaleButtonListener(Action<int> OnPush)
    {
        foreach (var sb in ScaleButtons)
            sb.OnPush = OnPush;
    }

    public void SetUniqueButton(Action OnPush, UniqueMenuButtonType Type)
    {
        var btn = GetUniqueButton(Type);
        btn.OnPush = OnPush;
    }

    private UniqueMenuButton GetUniqueButton(UniqueMenuButtonType type)
    {
        return UniqueButtons.FirstOrDefault(b => b.ButtonType == type);
    }


    public IEnumerator OpenColorPalette(float time)
    {
        if (isMovingPalette)
            yield break;
        isMovingPalette = true;
        IsOpendColorPalette = true;
        ColorPaletteAnime.Play(ColorPaletteMoveInHash, 0, 0);
        //normarizedTimeの0化は1F後におこなわれるので一旦待つ
        yield return null;

        while (ColorPaletteAnime.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            yield return null;
        isMovingPalette = false;
        PaletteMoveButtonText.text = "▽";
    }
    public IEnumerator CloseColorPalette(float time)
    {
        if (isMovingPalette)
            yield break;
        isMovingPalette = true;
        ColorPaletteAnime.Play(ColorPaletteMoveOutHash, 0, 0);
        //normarizedTimeの0化は1F後におこなわれるので一旦待つ
        yield return null;

        while (ColorPaletteAnime.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            yield return null;
        isMovingPalette = false;
        IsOpendColorPalette = false;
        PaletteMoveButtonText.text = "△";
    }
}
