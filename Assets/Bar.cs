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
        initialScale = bar.transform.localScale; 
        AnimateBar();
    }
    public void AnimateBar()
    {
        LeanTween.scaleX(bar, 1, time).setOnComplete(GameUpdate);
    }
    void GameUpdate()
    {
        a = GameObject.FindGameObjectWithTag("GameBoard").GetComponent<GameBoard>();
        a.NextTurn();

   
        bar.transform.localScale = initialScale;

  
        AnimateBar();
    }
}
