using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using TMPro;
using UnityEngine.SceneManagement;

public class MultiplayerController : MonoBehaviour
{
    #region Single Instance DontDestroyOnLoad

    public static MultiplayerController Instance { set; get; }
    public Server server;
    public Client client;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        server = gameObject.GetComponent<Server>();
        client = gameObject.GetComponent<Client>();

        RegisterEvents();
    }
    #endregion

    // UI Menus from the multiplayer lobby
    [SerializeField] private GameObject hostMenu;
    [SerializeField] private GameObject joinMenu;
    [SerializeField] private GameObject startMenu;
    [SerializeField] private TMP_Text startText;

    [SerializeField] private TMP_InputField addressInput;
    public int playerCount = -1;    // Used only for server interactions
    public int currentTeam = -1;    // What team has the user been assigned

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

    IEnumerator OnGameStart()
    {
        hostMenu.SetActive(false);
        joinMenu.SetActive(false);
        startMenu.SetActive(true);

        if (currentTeam == 0)
            startText.text = "The game will begin shortly.\n\nYou are playing as the Imperial Navy.";
        else
            startText.text = "The game will begin shortly.\n\nYou are playing as the Space Pirates.";        

        yield return new WaitForSeconds(5f);

        //startMenu.SetActive(false);

        SceneManager.LoadScene("Piece Selection");
    }

    public void DeleteMultiplayerInstance()
    {
        Destroy(gameObject);
    }

    #region Events
    private void RegisterEvents()
    {
        NetUtility.S_WELCOME += OnWelcomeServer;

        NetUtility.C_WELCOME += OnWelcomeClient;

        NetUtility.C_START_GAME += OnStartGameClient;
    }

    

    private void UnRegisterEvents()
    {

    }

    // Server
    private void OnWelcomeServer(NetMessage msg, NetworkConnection cnn)
    {
        NetWelcome nw = msg as NetWelcome;

        // Assigns player 1 a team randomly
        // 0 = Navy, 1 = Pirates
        if(currentTeam == -1)
        {
            playerCount++;
            nw.AssignedTeam = Random.Range(0, 2);
        }
        // Assigns player 2 whatever team is left
        else
        {
            playerCount++;
            nw.AssignedTeam = 1 - currentTeam;
        }

        // Returns back to client
        Server.Instance.SendToClient(cnn, nw);

        // Start the game when the connection is full
        if(playerCount == 1)
        {
            Debug.Log("Server broadcast startgame msg");
            Server.Instance.Broadcast(new NetStartGame());
        }
    }
    
    // Client
    private void OnWelcomeClient(NetMessage msg)
    {
        // Receive connection message
        NetWelcome nw = msg as NetWelcome;

        // Assigns team
        currentTeam = nw.AssignedTeam;
        Debug.Log($"My assigned team is {nw.AssignedTeam}");
    }
    
    private void OnStartGameClient(NetMessage msg)
    {
        Debug.Log("Starting the game");
        // Runs the start sequence
        StartCoroutine(OnGameStart());
    }
    #endregion
}
