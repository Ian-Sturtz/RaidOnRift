// Made by Garrett Slattengren

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Story : MonoBehaviour
{
    [SerializeField] private TMP_Text lore;

    public void NavyClicked()
    {
        lore.SetText("Navy lore here.");
    }

    public void PirateClicked()
    {
        lore.SetText("Pirate lore here.");
    }

    public void NavyCrewClicked()
    {
        lore.SetText("Navy crew lore here.");
    }

    public void PirateCrewClicked()
    {
        lore.SetText("Pirate crew lore here.");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
