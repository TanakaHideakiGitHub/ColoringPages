﻿using Tanaka;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public MenuView View;
    public Painter Painter;

    void Start ()
    {
        SetColorButtonPush();
        SetScaleButtonPush();
        SetUniqueButtonPush();
    }
	
    void SetColorButtonPush()
    {
        View.SetColorButtonListener(OnColorPush);
    }
    void SetScaleButtonPush()
    {
        View.SetScaleButtonListener(Painter.ChangeScale);
    }
    void SetUniqueButtonPush()
    {
        View.SetUniqueButton(OnAllEraze, UniqueMenuButtonType.AllEraze);
        View.SetUniqueButton(PulletMove, UniqueMenuButtonType.PulletMove);
        View.SetUniqueButton(Painter.OnSaveButton, UniqueMenuButtonType.Save);
        View.SetUniqueButton(Painter.OpenImageScroller, UniqueMenuButtonType.OpenScroller);
    }

    private void OnColorPush(Color col)
    {
        Painter.ColorChange(col);
        PulletMove();
    }
    private void OnAllEraze()
    {
        Painter.CreatePage();
        PulletMove();
    }

    private void PulletMove()
    {
        StartCoroutine(PulletMoceCoroutine());
    }

    private IEnumerator PulletMoceCoroutine()
    {
        if (!View.IsOpendColorPullet)
        {
            Painter.IsUnpaintable = true;
            yield return StartCoroutine(View.OpenColorPullet(0.5f));
        }
        else
        {
            yield return StartCoroutine(View.CloseColorPullet(0.5f));
            Painter.IsUnpaintable = false;
        }
    }
}