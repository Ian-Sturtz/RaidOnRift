using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ConnectionDroppedMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text disconnectMessage;
    [SerializeField] private bool gameWon;

    private void Awake()
    {
        MultiplayerController.Instance.StopAllCoroutines();

        if(MultiplayerController.Instance.gameWon == 1)
        {
            disconnectMessage.text = "Congratulations! You have won the game!\n\nPlease wait and you will be taken to the main menu...";
        }
        else if(MultiplayerController.Instance.gameWon == 0)
        {
            disconnectMessage.text = "Too bad! You have lost the game!\n\nPlease wait and you will be taken to the main menu...";
        }
        else
        {
            disconnectMessage.text = "Your connection has ended.\n\nPlease wait and you will be taken to the main menu...";
        }
    }

    private void Start()
    {
        Debug.Log("The Connection Dropped scene is managing server interactions");
        Server.Instance.Shutdown();
        Client.Instance.Shutdown();
        StartCoroutine(DisplayConnectionDropped());
    }

    IEnumerator DisplayConnectionDropped()
    {
        for (int i = 0; i < 3; i++)
        {
            Debug.Log("Waiting for seconds");
            yield return new WaitForSeconds(1f);
        }

        SceneManager.LoadScene("Main Menu");
    }
}
