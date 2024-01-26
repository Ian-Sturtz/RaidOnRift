// Implemented by Garrett Slattengren

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayLocal()
    {
        SceneManager.LoadScene("Board");
    }

    public void PlayLan()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void ViewLore()
    {
        SceneManager.LoadScene("Story");
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Closing game");
        Application.Quit();
    }
}
