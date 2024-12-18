using UnityEngine;
using Unity.Networking.Transport;
using System;
using Unity.Collections;

public class Client : MonoBehaviour
{
    #region Singleton Implementation
    public static Client Instance { set; get; }
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public NetworkDriver driver;
    private NetworkConnection connection;

    public bool isActive = false;
    public Action connectionDropped;

    // Methods
    public void Init(string ip, ushort port)
    {
        // Initializes the driver with a timeout of 25 seconds
        // Keepalives are sent every 20 seconds
        var settings = new NetworkSettings();
        settings.WithNetworkConfigParameters(disconnectTimeoutMS: 25000);
        driver = NetworkDriver.Create(settings);

        NetworkEndpoint endpoint = NetworkEndpoint.Parse(ip, port);
        Debug.Log("Attempting to connect to server at " + endpoint.Address);
        connection = driver.Connect(endpoint);

        if(connection == default(NetworkConnection))
        {
            Debug.Log("Connection failed");
        }

        isActive = true;

        RegisterToEvent();
    }

    public void Shutdown()
    {
        if (isActive)
        {
            Debug.Log("Shutting down client now");
            UnregisterToEvent();

            ClientDisconnect();

            driver.Dispose();
            isActive = false;
            connection = default(NetworkConnection);
        }
        else
        {
            Debug.Log("Server is already shut down");
        }
    }

    public void ClientDisconnect()
    {
        Debug.Log("Client is disconnecting");
        connection.Disconnect(driver);
        driver.ScheduleUpdate(default).Complete();
        Debug.Log("Client has disconnected");
    }

    public void OnDestroy()
    {
        Shutdown();
    }

    public void Update()
    {
        if (!isActive)
        {
            return;
        }

        driver.ScheduleUpdate().Complete();
        CheckAlive();
        UpdateMessagePump();
    }

    private void CheckAlive()
    {
        if(!connection.IsCreated && isActive)
        {
            Debug.Log("Connection to server lost.");
            connectionDropped?.Invoke();
            Shutdown();
        }
    }

    private void UpdateMessagePump()
    {
        DataStreamReader stream;
        NetworkEvent.Type cmd;

        try
        {
            while ((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Connect)
                {
                    SendToServer(new NetWelcome());
                    Debug.Log("Connected to server!");
                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    NetUtility.OnData(stream, default(NetworkConnection));
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client got disconnected from server");
                    connection = default(NetworkConnection);
                    connectionDropped?.Invoke();
                    Shutdown();
                    MultiplayerController.Instance.ConnectionDropped();

                }
            }
        }
        catch (ObjectDisposedException)
        {
            Debug.LogError("Attempted to use a disposed network resource.");
            throw;
        }
    }

    public void SendToServer(NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }

    // Event parsing
    private void RegisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE += OnKeepAlive;
    }

    private void UnregisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE -= OnKeepAlive;
    }

    private void OnKeepAlive(NetMessage nm)
    {
        //Send message back to keep alive
        Debug.Log("Received keepalive from server");
        SendToServer(nm);
    }
}