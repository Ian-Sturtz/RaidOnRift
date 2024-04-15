using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionDroppedMenu : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        StopAllCoroutines();
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
