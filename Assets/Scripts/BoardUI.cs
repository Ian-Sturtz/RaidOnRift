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



    public void UpdateTurn(bool navyTurn)
    {
        if(navyTurn)
        {
            turnDisplay.SetText("Navy's Turn");
            turnDisplay.color = new UnityEngine.Color (0, 0.03921569f, 0.6666667f, 1);
        }
        else
        {
            turnDisplay.SetText("Pirate's Turn");
            turnDisplay.color = new UnityEngine.Color (0.4588234f, 0f, 0f, 1f);
        }
    }

    public void GameWon(bool navyWon)
    {
        GoalText("The game is over");
        if (navyWon)
        {
            turnDisplay.SetText("Navy has Won!");
        }
        else
        {
            turnDisplay.SetText("Pirates have Won!");
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

    public void DisplayTempText(string text, float delay = 1f)
    {
        StopAllCoroutines();
        temporaryTextActive = true;
        StartCoroutine(TemporaryText(text, delay));
    }

    // Sets the goal text to the provided text, or appends the provided text onto an existing goal text
    public void GoalText(string text, bool append = false)
    {
        temporaryTextActive = false;
        StopAllCoroutines();

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

    public void LoadMenu()
    {

    }
}
