using Tanaka;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public MenuView View;
    public Painter Painter;

    public bool IsOpend { get { return View.IsOpendColorPalette; } }

    void Start ()
    {
        SetColorButtonPush();
        SetScaleButtonPush();
        SetUniqueButtonPush();
    }

    private void SetColorButtonPush()
    {
        View.SetColorButtonListener(OnColorPush);
    }
    private void SetScaleButtonPush()
    {
        View.SetScaleButtonListener(Painter.ChangeScale);
    }
    private void SetUniqueButtonPush()
    {
        View.SetUniqueButton(OnAllEraze, UniqueMenuButtonType.AllEraze);
        View.SetUniqueButton(PaletteMove, UniqueMenuButtonType.PaletteMove);
        View.SetUniqueButton(Painter.OnSaveButton, UniqueMenuButtonType.Save);
        View.SetUniqueButton(Painter.OpenImageScroller, UniqueMenuButtonType.OpenScroller);
    }

    private void OnColorPush(Color col)
    {
        Painter.ColorChange(col);
        PaletteMove();
    }
    private void OnAllEraze()
    {
        Painter.CreatePage();
        PaletteMove();
    }

    private void PaletteMove()
    {
        StartCoroutine(PaletteMoceCoroutine());
    }

    private IEnumerator PaletteMoceCoroutine()
    {
        if (!View.IsOpendColorPalette)
            yield return StartCoroutine(View.OpenColorPalette(0.5f));
        else
            yield return StartCoroutine(View.CloseColorPalette(0.5f));
    }
}
