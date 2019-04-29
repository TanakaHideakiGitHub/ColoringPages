using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : SingletonMonoBehaviour<SceneLoader>
{
    private static readonly string SCENES_IMAGES_RESOURCES_PATH = "Textures/SceneScreenShots";

    private enum SceneName
    {
        BaseScene,
        ColoringPage,
        PictureBook1,
        PictureBook2,
    }

    public SceneView SceneView;

    private ImageScrollController imageScrollController;
    private int preSceneIndex;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
        //起動時にお絵描きシーンに遷移させておく
        LoadScene((int)SceneName.ColoringPage);
    }

    void Start()
    {
        imageScrollController = ImageScrollController.Instance;
        SceneView.OnPushSceneChange(ChangeScene);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void LoadScene(int idx)
    {
        SceneManager.LoadSceneAsync(idx, LoadSceneMode.Additive);
    }

    private void ChangeScene()
    {
        if (imageScrollController.IsOpend)
        {
            imageScrollController.Close();
            return;
        }

        var scene = SceneManager.GetActiveScene();
        preSceneIndex = scene.buildIndex;
        int idx = 0;
        if (scene.buildIndex == (int)SceneName.ColoringPage)
        {
            imageScrollController.Open(SCENES_IMAGES_RESOURCES_PATH, OnNodePush);
            return;
        }
        else
            idx = (int)SceneName.ColoringPage;
        LoadScene(idx);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(scene);
        if (preSceneIndex != (int)SceneName.BaseScene)
            SceneManager.UnloadSceneAsync(preSceneIndex);
    }

    /// <summary>
    /// スクロールのノード押下時
    /// </summary>
    /// <param name="tex"></param>
    private void OnNodePush(Texture2D tex)
    {
        Debug.Log(tex.name);
        SceneManager.LoadSceneAsync(tex.name, LoadSceneMode.Additive);
    }
}
