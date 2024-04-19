using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour
{
    [SerializeField] private GameObject Timer;
    [SerializeField] private TMP_Text CountdownClock;
    [SerializeField] private float timeRemaining = 75f;
    [SerializeField] private Color clockTimer;
    [SerializeField] private Color panicTimer;
    bool timerIsRunning = false;
    public bool piecesSent { set; get; }

    private void Start()
    {
        piecesSent = false;
    }

    public void startTimer()
    {
        // Starts a countdown clock of 3 minutes
        StartCoroutine(PieceSelectionTimer());
    }

    public void stopTimer()
    {
        StopCoroutine(PieceSelectionTimer());
    }

    // Waits for a certain length of time before dropping the connection
    IEnumerator PieceSelectionTimer()
    {
        timerIsRunning = true;

        while (timerIsRunning)
        {
            if (timeRemaining > 0)
                timeRemaining -= Time.deltaTime;
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
            }

            displayTimeRemaining();
            yield return null;
        }

        if (timeRemaining == 0)
        {
            if (piecesSent)
                MultiplayerController.Instance.gameWon = 1;
            else
                MultiplayerController.Instance.gameWon = 0;

            MultiplayerController.Instance.ConnectionDropped();
        }
    }

    private void displayTimeRemaining()
    {
        float minutes = Mathf.FloorToInt(timeRemaining / 60);
        float seconds = Mathf.FloorToInt(timeRemaining % 60);

        CountdownClock.text = string.Format("{0:0}:{1:00}", minutes, seconds);

        if (minutes < 1)
            CountdownClock.color = panicTimer;
        else
            CountdownClock.color = clockTimer;
    }
}
