using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

public class Server : MonoBehaviour
{
    #region Singleton Implementation
    public static Server Instance { set; get; }
    private void Awake()
    {
        Instance = this;
    }
    #endregion
}