using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public void LoadMainMenu()
    {
        if (PieceManager.instance.onlineMultiplayer)
        {
            MultiplayerController.Instance.gameWon = 0;

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
        
        if(PieceManager.instance.onlineMultiplayer)
        {
            MultiplayerController.Instance.gameWon = 0;

            Client.Instance.Shutdown();
            Server.Instance.Shutdown();
        }

        Application.Quit();
    }
}
