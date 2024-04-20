using UnityEngine;
using UnityEngine.UI;  // For UGUI
using TMPro;  // For TextMeshPro
using System.Net;
using System.Linq;

public class DisplayIP : MonoBehaviour
{
    public TextMeshProUGUI ipTextTMP;  
    

    void Start()
    {
        ipTextTMP.text = "Your IP Address: " + GetLocalIPAddress();
        
    }

    string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "Local IP Address Not Found";
    }
}