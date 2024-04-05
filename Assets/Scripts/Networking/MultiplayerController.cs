using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiplayerController: MonoBehaviour
{
    #region Single Instance DontDestroyOnLoad
    public static MultiplayerController MP_instance;
    public Server server;
    public Client client;

    private void Awake()
    {
        if (MP_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        MP_instance = this;
        DontDestroyOnLoad(gameObject);
        server = gameObject.GetComponent<Server>();
        client = gameObject.GetComponent<Client>();
    }
    #endregion

    [SerializeField] private TMP_InputField addressInput;

    public void OnOnlineHostButton()
    {
        server.Init(8007);
        client.Init("127.0.0.1", 8035);
    }

    public void OnOnlineConnectButton()
    {
        Debug.Log("Attempting to connect to " + addressInput.text);
        client.Init(addressInput.text, 8035);
    }

    public void OnOnlineHostBack()
    {
        server.Shutdown();
        client.Shutdown();
    }

    public void OnOnlineJoinBack()
    {
        client.Shutdown();
    }
}
