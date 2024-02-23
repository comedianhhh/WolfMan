using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    public System.Action<List<string>> OnSceneLoadedEvent;

    public float delayTime = 1.0f;

    private List<string> loadedScenes = new List<string>();

    // When loading just add a flag for persistence. If true don't add to the loadedScenes
    // Only remove the scenes when you unload

    public void LoadScene(string scene, bool showLoadingScreen = true)
    {
        StartCoroutine(loadScene(scene, showLoadingScreen, true));
    }

    public void LoadScenes(List<string> scenes, bool showLoadingScreen = true)
    {
        StartCoroutine(loadScenes(scenes, showLoadingScreen));
    }

    IEnumerator loadScenes(List<string> scenes, bool showLoadingScreen)
    {
        if (showLoadingScreen)
        {
            MenuManager.Instance.ShowMenu(MenuManager.Instance.LoadingScreenClassifier);
        }

        foreach (string scene in scenes)
        {
            yield return StartCoroutine(loadScene(scene, false, false));
        }

        if (showLoadingScreen)
        {
            MenuManager.Instance.HideMenu(MenuManager.Instance.LoadingScreenClassifier);
        }

        loadedScenes.Clear();
        loadedScenes.AddRange(scenes);
        OnSceneLoadedEvent?.Invoke(loadedScenes);
        Debug.Log("Switch");
    }

    IEnumerator loadScene(string scene, bool showLoadingScreen, bool raiseEvent)
    {
        if (SceneManager.GetSceneByPath(scene).isLoaded == false)
        {
            if (showLoadingScreen)
            {
                MenuManager.Instance.ShowMenu(MenuManager.Instance.LoadingScreenClassifier);
            }

            yield return new WaitForSeconds(delayTime);

            AsyncOperation sync;

            Application.backgroundLoadingPriority = ThreadPriority.Low;

            sync = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            while (sync.isDone == false) { yield return null; }

            Application.backgroundLoadingPriority = ThreadPriority.Normal;

            yield return new WaitForSeconds(delayTime);

            if (showLoadingScreen)
            {
                MenuManager.Instance.HideMenu(MenuManager.Instance.LoadingScreenClassifier);
            }
        }

        if (raiseEvent)
        {
            loadedScenes.Clear();
            loadedScenes.Add(scene);
            OnSceneLoadedEvent?.Invoke(loadedScenes);

        }
    }

    // 4 Methods:
    //	- Unload single scene
    //	- Unload list of scenes
    //		- Support to unload multiple (Coroutine)
    //	- Actual Unload of scenes.

    public void UnloadScene(string scene)
    {
        StartCoroutine(unloadScene(scene));
    }



    IEnumerator unloadScenes(List<string> scenes)
    {
        foreach (string scene in scenes)
        {
            yield return StartCoroutine(unloadScene(scene));
        }
    }

    IEnumerator unloadScene(string scene)
    {
        AsyncOperation sync = null;

        try
        {
            sync = SceneManager.UnloadSceneAsync(scene);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        if (sync != null)
        {
            while (sync.isDone == false) { yield return null; }
        }

        sync = Resources.UnloadUnusedAssets();
        while (sync.isDone == false) { yield return null; }
    }
    public void LoadScene(string scene, Action callback = null, bool showLoadingScreen = true)
    {
        StartCoroutine(LoadSceneCoroutine(scene, showLoadingScreen, callback));
    }

    private IEnumerator LoadSceneCoroutine(string scene, bool showLoadingScreen, Action callback)
    {
        if (showLoadingScreen)
        {
            MenuManager.Instance.ShowMenu(MenuManager.Instance.LoadingScreenClassifier);
        }

        // Begin loading the scene asynchronously.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (showLoadingScreen)
        {
            MenuManager.Instance.HideMenu(MenuManager.Instance.LoadingScreenClassifier);
        }

        // Scene has loaded, now invoke the callback.
        callback?.Invoke();
    }
    public void UnloadScenes(List<string> scenes)
    {
        StartCoroutine(UnloadScenesCoroutine(scenes));
    }

    private IEnumerator UnloadScenesCoroutine(List<string> scenes)
    {
        foreach (string scene in scenes)
        {
            yield return StartCoroutine(UnloadSceneCoroutine(scene));
        }
    }

    private IEnumerator UnloadSceneCoroutine(string scene)
    {
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);
        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        // Optionally, unload unused assets to free up memory.
        yield return Resources.UnloadUnusedAssets();
    }
    // Other methods (e.g., UnloadScene) remain unchanged
}

