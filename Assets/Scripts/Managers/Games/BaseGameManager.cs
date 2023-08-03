using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseGameManager : MonoBehaviour
{
    public Button startButton;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI countdownText;

    private double timerStart;
    private double countdownStart;
    private bool timerRunning = false;
    private bool counting = false;

    private double defaultMinigameTime = 180;

    public enum GameState
    {
        preparing = 0,
        countingDown = 1,
        playing = 2,
        finished = 3,
    }
    public GameState state;

    public virtual void Start()
    {
        timerText.enabled = false;
        countdownText.enabled = false;
        state = GameState.preparing;
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.preparing:
                PreparingLoop();
                break;
            case GameState.countingDown:
                CountdownLoop();
                break;
            case GameState.playing:
                GameLoop();
                break;
            case GameState.finished:
                FinishedLoop();
                break;
        }
    }

    public virtual void StartGame()
    {
        startButton.enabled = false;
        countdownText.enabled = true;
        state = GameState.countingDown;
        countdownStart = NetworkTime.time;
    }

    /// <summary>
    /// For executing before the minigame starts
    /// </summary>
    public virtual void PreparingLoop() { }

    /// <summary>
    /// For executing the countdown to start the minigame.
    /// </summary>
    public virtual void CountdownLoop() {
        if (!counting)
        {
            StartCoroutine(StartCountdown(3));
        }
        //// In case coroutines are not network-safe
        //double countdown = 3.5 - (NetworkTime.time - countdownStart);
        //countdownText.text = countdown.ToString("N0");

        //if (countdown < 0.5)
        //{
        //    state = GameState.playing;
        //    countdownText.text = "START";
        //}
    }

    /// <summary>
    /// For executing the minigame's core gameplay loop.
    /// </summary>
    public virtual void GameLoop()
    {
        timerText.enabled = true;
        if (!timerRunning)
        {
            timerStart = NetworkTime.time;
            StartCoroutine(StartTimer(defaultMinigameTime));
        }
    }

    /// <summary>
    /// For executing after the minigame finishes
    /// </summary>
    public virtual void FinishedLoop() { }

    public IEnumerator StartCountdown(double countdownMax)
    {
        counting = true;
        double countdown = countdownMax;
        while (countdown > 0.5)
        {
            countdown = countdownMax - (NetworkTime.time - countdownStart);
            countdownText.text = countdown.ToString("N0");
            yield return null;
        }

        // After countdown finishes
        countdownText.text = "Start";
        yield return new WaitForSeconds(1);
        state = GameState.playing;
        countdownText.enabled = false;
        counting = false;
    }

    public virtual IEnumerator StartTimer(double timerMax)
    {
        timerRunning = true;
        double timer = timerMax;
        while (timer > 0)
        {
            timer = timerMax - (NetworkTime.time - timerStart);
            timerText.text = timer.ToString("N2");
            yield return null;
        }

        state = GameState.finished;
        timerRunning = false;
    }
}
