// Implemented by Garrett Slattengren

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text turnDisplay;
    [SerializeField] private TMP_Text goalDisplay;
    private bool navyHasOre = false;
    private bool pirateHasOre = false;

    public void UpdateTurn(bool navyTurn)
    {
        if(navyTurn)
        {
            turnDisplay.SetText("Navy's Turn");

            if(navyHasOre)
            {
                goalDisplay.SetText("Bring your orebearer back to your side of the board");
            }
            else
            {
                goalDisplay.SetText("Capture your opponent's ore");
            }
        }
        else
        {
            turnDisplay.SetText("Pirate's Turn");

            if (pirateHasOre)
            {
                goalDisplay.SetText("Bring your orebearer back to your side of the board");
            }
            else
            {
                goalDisplay.SetText("Capture your opponent's ore");
            }
        }
    }

    public void GameWon(bool navyWon)
    {
        if(navyWon)
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

    public void LoadMenu()
    {

    }
}
