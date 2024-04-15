using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public void LoadMainMenu()
    {
        if (MultiplayerController.Instance != null)
        {
            Client.Instance.Shutdown();
            Server.Instance.Shutdown();
            SceneManager.LoadScene("Connection Dropped");
        }
            
        else
            SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        
        if(MultiplayerController.Instance != null)
        {
            Client.Instance.Shutdown();
            Server.Instance.Shutdown();
        }

        Application.Quit();
    }
}
