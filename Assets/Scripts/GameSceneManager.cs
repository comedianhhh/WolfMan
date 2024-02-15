using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class GameSceneManager : MonoBehaviour
{
    public TMP_Text tmScore;
    public TMP_Text tmHighScore;
    public TMP_Text tmTimeLeft;

    public TMP_Text tmLevel;

    private int MAX_TIME = 45;
    private int SCORE_INCREMENT = 10;
    private int currentTime;
    public int currentScore;
    private int highScore;

    public Canvas GameOverCanvas;
    public Canvas MainCanvas;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Game Begins");

        currentTime = MAX_TIME;

        if (PlayerPrefs.HasKey("HighScore"))
            highScore = PlayerPrefs.GetInt("HighScore");
        else
            highScore = 0;

        
        tmHighScore.text = "High Score: " + highScore;
        tmLevel.text = "Level: 1";

        StartCoroutine("LoseTime");
    }

    private void UpdateLabels()
    {
        tmScore.text = "Score: " + currentScore;
        tmTimeLeft.text = "Time Left: " + currentTime;
    }

    IEnumerator LoseTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            currentTime--;

            if (currentTime % 5 == 0)
                currentScore += SCORE_INCREMENT;

            if (currentTime <= 0)
                break;
        }

        // game over
        GameOver();
    }

    private void GameOver()
    {
        if (highScore < currentScore)
            PlayerPrefs.SetInt("HighScore", currentScore);

        Debug.Log("Game Over");
        StartCoroutine(FadeInCanvas());
        StartCoroutine(WaitDelayFadeOutCanvas(3.0f));

    }
    // Update is called once per frame
    void Update()
    {
        UpdateLabels();

    }

    public IEnumerator FadeInCanvas()
    {

        yield return StartCoroutine(FadeEffect.FadeCanvas(GameOverCanvas.GetComponent<CanvasGroup>(), 0f, 1f, 1f));
    }

    public IEnumerator WaitDelayFadeOutCanvas(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StartCoroutine(FadeOutCanvas());
    }

    public IEnumerator FadeOutCanvas()
    {
        yield return StartCoroutine(FadeEffect.FadeCanvas(MainCanvas.GetComponent<CanvasGroup>(), 1f, 0f, 1f));
    }
}
