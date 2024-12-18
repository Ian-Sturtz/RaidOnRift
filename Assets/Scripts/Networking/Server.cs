using Unity.Collections;
using UnityEngine;
using Unity.Networking.Transport;
using System;

public class Server : MonoBehaviour
{
    #region Singleton Implementation
    public static Server Instance { set; get; }

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public NetworkDriver driver;
    [SerializeField] private NativeList<NetworkConnection> connections;

    public bool isActive = false;
    private const float keepAliveTickRate = 20f;    // KeepAlives are sent every 20 seconds
    private float lastKeepAlive;                    // Timeout rate is set to 25 seconds

    public Action connectionDropped;

    // Methods
    public void Init(ushort port)
    {
        // Initializes the driver with a timeout of 25 seconds
        // Keepalives are sent every 20 seconds
        var settings = new NetworkSettings();
        settings.WithNetworkConfigParameters(disconnectTimeoutMS: 25000);
        driver = NetworkDriver.Create(settings);

        NetworkEndpoint endpoint = NetworkEndpoint.AnyIpv4;
        endpoint.Port = port;

        if (driver.Bind(endpoint) != 0)
        {
            Debug.Log("Unable to bind to port " + endpoint.Port);
            return;
        }
        else
        {
            Debug.Log("Currently listening to port " + endpoint.Port);
            driver.Listen();
        }

        connections = new NativeList<NetworkConnection>(2, Allocator.Persistent);
        isActive = true;
    }

    public void Shutdown()
    {
        if (isActive)
        {
            ServerDisconnect();

            connections.Dispose();
            driver.Dispose();
            isActive = false;
        }
        else
        {
            Debug.Log("Server is already shut down");
        }
    }

    public void ServerDisconnect()
    {
        Debug.Log("Server is disconnecting");

        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i] != default(NetworkConnection))
            {
                connections[i].Disconnect(driver);
                driver.ScheduleUpdate(default).Complete();
            }
        }

        Debug.Log("Server has disconnected");
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

        KeepAlive();

        driver.ScheduleUpdate().Complete();

        Cleanupconnections();
        AcceptNewconnections();
        UpdateMessagePump();
    }

    private void KeepAlive()
    {
        if(Time.time - lastKeepAlive > keepAliveTickRate)
        {
            lastKeepAlive = Time.time;
            Broadcast(new NetKeepAlive());
        }
    }

    private void Cleanupconnections()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                i--;
            }
        }
    }

    private void AcceptNewconnections()
    {
        NetworkConnection c;

        while ((c = driver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c);
        }
    }

    private void UpdateMessagePump()
    {
        DataStreamReader stream;

        for (int i = 0; i < connections.Length; i++)
        {
            try
            {
                if (connections[i] != default(NetworkConnection) && isActive)
                {
                    NetworkEvent.Type cmd;
                    while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
                    {
                        if (cmd == NetworkEvent.Type.Data)
                        {
                            NetUtility.OnData(stream, connections[i], this);
                        }
                        else if (cmd == NetworkEvent.Type.Disconnect)
                        {
                            Debug.Log("Client disconnected from server");
                            connections[i] = default(NetworkConnection);
                            connectionDropped?.Invoke();
                            Shutdown();
                            MultiplayerController.Instance.ConnectionDropped();
                        }
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                Debug.LogError("Attempted to use a disposed network resource.");
            }
        }
    }

    public void SendToClient(NetworkConnection connection, NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }

    public void Broadcast(NetMessage msg)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i].IsCreated)
            {
                Debug.Log($"Sending {msg.Code} to : {i}");
                SendToClient(connections[i], msg);
            }
        }
    }
}