using UnityEngine;
using TMPro; 
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Linq;

public class DisplayIP : MonoBehaviour
{
    public TextMeshProUGUI ipTextTMP;

    void Start()
    {
        ipTextTMP.text = "Your IP Address: " + GetPreferredIPAddress();
    }

    private string GetPreferredIPAddress()
    {
        string ipAddress = "Local IP Address Not Found";

      
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
            .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
            .Where(ni => ni.GetIPProperties().GatewayAddresses.Any())
            .Where(ni => ni.GetIPProperties().UnicastAddresses.Any(ua => ua.Address.AddressFamily == AddressFamily.InterNetwork));

        
        var preferredInterface = networkInterfaces.OrderByDescending(ni => ni.Speed).FirstOrDefault();

        if (preferredInterface != null)
        {
        
            var ipv4Address = preferredInterface.GetIPProperties().UnicastAddresses
                .FirstOrDefault(ua => ua.Address.AddressFamily == AddressFamily.InterNetwork)?.Address.ToString();

            if (ipv4Address != null)
                ipAddress = ipv4Address;
        }

        return ipAddress;
    }
}
