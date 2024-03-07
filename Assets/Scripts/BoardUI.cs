// Implemented by Garrett Slattengren

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class BoardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text turnDisplay;
    [SerializeField] private TMP_Text goalDisplay;
    private bool temporaryTextActive = false;
    private bool navyHasOre = false;
    private bool pirateHasOre = false;

    private bool navyTurn = true;
    private bool gameOver = false;

    [SerializeField] private GameObject animTextObj;   
    [SerializeField] private TMP_Text animText;
    [SerializeField] private GameObject animBackground;

    private void Update()
    {
        if (navyTurn && !gameOver) { 
        
            turnDisplay.SetText("NAVY'S TURN");
            turnDisplay.color = new UnityEngine.Color(0, 0.03921569f, 0.6666667f, 1);
        }
        else if(!gameOver)
        {
            turnDisplay.SetText("PIRATE'S TURN");
            turnDisplay.color = new UnityEngine.Color(0.4588234f, 0f, 0f, 1f);
        }
    }

    public void GameWon(bool navyWon, bool stalemate = false)
    {
        gameOver = true;

        GoalText("THE GAME IS OVER");
        if(navyWon && stalemate)
        {
            turnDisplay.SetText("STALEMATE! NAVY WINS!");
            turnDisplay.color = new UnityEngine.Color(0, 0.03921569f, 0.6666667f, 1);
        }else if (stalemate)
        {
            turnDisplay.SetText("STALEMATE! PIRATES WIN!");
            turnDisplay.color = new UnityEngine.Color(0.4588234f, 0f, 0f, 1f);
        }
        else if (navyWon)
        {
            turnDisplay.SetText("NAVY WINS!");
            turnDisplay.color = new UnityEngine.Color(0, 0.03921569f, 0.6666667f, 1);
        }
        else
        {
            turnDisplay.SetText("PIRATES WIN!");
            turnDisplay.color = new UnityEngine.Color(0.4588234f, 0f, 0f, 1f);
        }
    }

    public void UpdateGoal(bool navyTurn, bool hasOre)
    {
        if(navyTurn)
        {
            navyHasOre = hasOre;
        }
        else
        {
            pirateHasOre = hasOre;
        }
    }

    public void UpdateTurn(bool currentTurn)
    {
        navyTurn = currentTurn;
    }

    public void DisplayTempText(string text, float delay = 1f)
    {
        //StopAllCoroutines();
        temporaryTextActive = true;
        StartCoroutine(TemporaryText(text, delay));
    }

    // Sets the goal text to the provided text, or appends the provided text onto an existing goal text
    public void GoalText(string text, bool append = false)
    {
        temporaryTextActive = false;
        //StopAllCoroutines();

        if (!append)
        {
            goalDisplay.SetText(text);
        }
        else
        {
            string currentText = goalDisplay.text;

            if(currentText == "")
            {
                currentText = text;
            }
            else
            {
                currentText = currentText + "\n" + text;
            }

            goalDisplay.SetText(currentText);
        }
    }

    IEnumerator TemporaryText(string temporaryText, float duration = 1f)
    {
        string currentText = goalDisplay.text;

        goalDisplay.SetText(temporaryText);
        yield return new WaitForSeconds(duration);
        goalDisplay.SetText(currentText);
        temporaryTextActive = false;
    }

    public void PlayTurnAnim(bool turn)
    {
        StopAllCoroutines();

        if (turn)
        {

            animText.SetText("NAVY'S TURN");
            animText.color = new UnityEngine.Color(0, 0.03921569f, 0.6666667f, 1);
        }
        else
        {
            animText.SetText("PIRATE'S TURN");
            animText.color = new UnityEngine.Color(0.4588234f, 0f, 0f, 1f);
        }
        StartCoroutine(AnimBackground());
        StartCoroutine(AnimText());
    }

    IEnumerator AnimBackground()
    {
        animBackground.SetActive(true);
        float timeElapsed = 0;
        float startHeight = animBackground.GetComponent<RectTransform>().rect.height;

        while (timeElapsed < 0.25f)
        {
            float val = Mathf.SmoothStep(startHeight, 250, timeElapsed / 0.25f);
            animBackground.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, val);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        animBackground.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, 250);
        yield return new WaitForSeconds(0.5f);

        timeElapsed = 0;
        while(timeElapsed < 0.25f)
        {
            float val = Mathf.SmoothStep(250, 0, timeElapsed / 0.25f);
            animBackground.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, val);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        animBackground.SetActive(false);
    }

    IEnumerator AnimText()
    {
        animTextObj.SetActive(true);
        float timeElapsed = 0;
        float red = animText.color.r;
        float green = animText.color.g;
        float blue = animText.color.b;
        //float alpha = animText.color.a;
        float alpha = 0;

        while (timeElapsed < 0.25f)
        {
            float val = Mathf.Lerp(alpha, 1, timeElapsed / 0.25f);
            animText.color = new UnityEngine.Color(red, green, blue, val);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        animText.color = new UnityEngine.Color(red, green, blue, 1);
        yield return new WaitForSeconds(0.5f);

        timeElapsed = 0;
        while (timeElapsed < 0.25f)
        {
            float val = Mathf.Lerp(1, 0, timeElapsed / 0.25f);
            animText.color = new UnityEngine.Color(red, green, blue, val);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        animTextObj.SetActive(false);
    }
}
