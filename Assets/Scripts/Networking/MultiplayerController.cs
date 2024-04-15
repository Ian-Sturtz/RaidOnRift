using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

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
            Destroy(Instance.gameObject);
            //return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        server = gameObject.GetComponent<Server>();
        client = gameObject.GetComponent<Client>();

        RegisterEvents();
    }
    #endregion

    // UI Menus from the multiplayer lobby
    [SerializeField] private GameObject lobbyMenu;
    [SerializeField] private GameObject hostMenu;
    [SerializeField] private GameObject joinMenu;
    [SerializeField] private GameObject startMenu;
    [SerializeField] private TMP_Text startText;

    [SerializeField] private TMP_InputField addressInput;
    // Used only for server interactions
    public int playerCount = -1;
    // What team has the user been assigned, 0: Navy, 1: Pirates
    public int currentTeam = -1;

    public int gameWon = -1;    // 0 for loss, 1 for win

    public void OnOnlineHostButton()
    {
        server.Init(8035);
        client.Init("127.0.0.1", 8035);
    }

    public void OnOnlineConnectButton()
    {
        Debug.Log("Attempting to connect to " + addressInput.text);

        if (IsValidIP(addressInput.text))
        {
            string inputIP = addressInput.text;
            client.Init(inputIP, 8035);
            addressInput.text = $"Connecting to {inputIP}";
        }
        else
        {
            Debug.Log("Invalid Input");
            addressInput.text = "";
        }
    }

    private bool IsValidIP(string ip)
    {
        string pattern = @"^(\d{1,3}\.){3}\d{1,3}$";
        Regex regex = new Regex(pattern);

        if (regex.IsMatch(ip))
        {
            string[] parts = ip.Split('.');
            foreach (string part in parts)
            {
                int intPart = int.Parse(part);
                if (intPart < 0 || intPart > 255)
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    public void OnOnlineHostBack()
    {
        server.Shutdown();
        client.Shutdown();
        playerCount = -1;
        currentTeam = -1;
    }

    public void OnOnlineJoinBack()
    {
        addressInput.text = "";
        client.Shutdown();
    }

    public void OnGameStartConfirmBack(bool messageSent = false)
    {
        if (!messageSent)
        {
            NetStartGame sg = new NetStartGame();
            sg.Start_Game = 0;
            Client.Instance.SendToServer(sg);
        }
        StopAllCoroutines();
        OnOnlineHostBack();

        SceneManager.LoadScene("Connection Dropped");
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

        SceneManager.LoadScene("Piece Selection");
    }

    public void ConnectionDropped()
    {
        Debug.Log("Connection has ended, routing user to appropriate menu");

        // Current Scene is Multiplayer Lobby
        // Inform player that the connection has dropped
        if(SceneManager.GetActiveScene().name == "Multiplayer Lobby")
        {
            SceneManager.LoadScene("Connection Dropped");
        }

        // Current Scene is Piece Selection or Piece Placement
        // Route players to Connection Dropped menu
            // Inform players the connection has dropped
            // Route them back to Multiplayer Lobby to attempt a reconnect
        if (SceneManager.GetActiveScene().name == "Piece Selection" || SceneManager.GetActiveScene().name == "Piece Placement")
        {
            SceneManager.LoadScene("Connection Dropped");
        }

        // Current Scene is the main game board
        // End the connection and allow the user to view the final game board, including who won
        if (SceneManager.GetActiveScene().name == "Board")
        {
            Debug.Log("Connection ended");
            SceneManager.LoadScene("Connection Dropped");
        }
    }

    public void DeleteMultiplayerInstance()
    {
        Instance = null;
        Destroy(gameObject);
    }

    #region Events
    private void RegisterEvents()
    {
        NetUtility.S_WELCOME += OnWelcomeServer;

        NetUtility.S_START_GAME += OnStartGameServer;

        NetUtility.C_WELCOME += OnWelcomeClient;

        NetUtility.C_START_GAME += OnStartGameClient;
    }

    private void UnRegisterEvents()
    {
        NetUtility.S_WELCOME -= OnWelcomeServer;

        NetUtility.C_WELCOME -= OnWelcomeClient;

        NetUtility.C_START_GAME -= OnStartGameClient;
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
            NetStartGame sg = new NetStartGame();
            sg.Start_Game = 1;
            Server.Instance.Broadcast(sg);
        }
    }
    
    private void OnStartGameServer(NetMessage msg, NetworkConnection cnn)
    {
        Server.Instance.Broadcast(msg);
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
        NetStartGame sg = msg as NetStartGame;

        if(sg.Start_Game == 1)
        {
            Debug.Log("Starting the game");
            // Runs the start sequence
            Instance.StartCoroutine(OnGameStart());
        }
        else
        {
            Debug.Log("Game Canceled");
            StopAllCoroutines();
            // Stops the start sequence and returns to game menu
            ConnectionDropped();
        }
    }
    #endregion
}
