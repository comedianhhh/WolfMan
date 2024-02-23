using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu
{
    public SceneReference LevelToLoad;
    public MenuClassifier hudClassifier;
    public GameObject sceneManager;
    public GameObject gameOver;

    public float volume = 1.0f;

    public void OnStartGame()
    {

        SceneLoader.Instance.LoadScene(LevelToLoad, true);
        MenuManager.Instance.HideMenu(menuClassifier);
        MenuManager.Instance.ShowMenu(hudClassifier);
        sceneManager.SetActive(true);
        gameOver.SetActive(true);
    }

    public void OnChangeVolume()
    {
        AudioManager.Instance.SetBackgroundVolume(volume);
    }
}
