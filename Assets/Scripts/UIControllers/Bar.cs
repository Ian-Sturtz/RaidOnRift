using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DentedPixel;
using TMPro;

public class Bar : MonoBehaviour
{
    [SerializeField] private GameObject bar;
    [SerializeField] private TMP_Text timerText;
    public int time;
    public bool timeOver = false;

    private Vector3 initialScale; 

    void Start()
    {
        if(MultiplayerController.Instance != null)
        {
            if(MultiplayerController.Instance.currentTeam == 0)
                timerText.color = new UnityEngine.Color(0, 0.03921569f, 0.6666667f, 1f);
            else
                timerText.color = new UnityEngine.Color(0.4588234f, 0f, 0f, 1f);
        }

        initialScale = bar.transform.localScale; 
        AnimateBar();
    }

    public void AnimateBar()
    {
        LeanTween.scaleX(bar, 0, time).setOnComplete(TimeOut);
    }

    public void ResetBar()
    {
        LeanTween.reset();
        bar.transform.localScale = initialScale;
        AnimateBar();
    }

    private void TimeOut()
    {
        timeOver = true;
    }

    public void pauseTimer()
    {
        if (PieceManager.instance != null)
        {
            if (!PieceManager.instance.onlineMultiplayer)
            {
                LeanTween.pause(bar);
            }
        }
        else
        {
            LeanTween.pause(bar);
        }
    }

    public void resumeTimer()
    {
        LeanTween.resumeAll();
    }
}
