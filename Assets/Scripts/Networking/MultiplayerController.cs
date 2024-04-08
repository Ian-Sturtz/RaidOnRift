using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using TMPro;

public class MultiplayerController: MonoBehaviour
{
    #region Single Instance DontDestroyOnLoad
    public static MultiplayerController MP_instance;
    public Server server;
    public Client client;

    // What team is the active player
    public int currentTeam = -1;

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

        RegisterEvents();
    }
    #endregion

    [SerializeField] private TMP_InputField addressInput;

    public void OnOnlineHostButton()
    {
        server.Init(8035);
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

    public void DeleteMultiplayerInstance()
    {
        Destroy(gameObject);
    }

    #region Events
    private void RegisterEvents()
    {
        NetUtility.S_WELCOME += OnWelcomeServer;
    }
    private void UnRegisterEvents()
    {

    }

    // Server
    private void OnWelcomeServer(NetMessage msg, NetworkConnection cnn)
    {
        NetWelcome nw = msg as NetWelcome;

        // Assign them a team randomly
        // 0 = Navy, 1 = Pirates
        nw.AssignedTeam = Random.Range(0, 2);

        Server.Instance.SendToClient(cnn, nw);
    }
    // Client
    #endregion
}
