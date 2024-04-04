using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DentedPixel;

public class Bar : MonoBehaviour
{
    public GameBoard a;

    public GameObject bar;
    public int time;

    private Vector3 initialScale; 


    void Start()
    {
        a = GameObject.FindGameObjectWithTag("GameBoard").GetComponent<GameBoard>();
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
        a.GameOver(!a.navyTurn);
    }

    public void pauseTimer()
    {
        //if(multiplayer)
        LeanTween.pause(bar);
    }

    public void resumeTimer()
    {
        LeanTween.resumeAll();
    }
}
