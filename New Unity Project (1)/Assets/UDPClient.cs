using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UDPClient : MonoBehaviour
{
    private UdpClient udpClient;

    public void Start()
    {
        udpClient = new UdpClient();
        udpClient.Connect(IPAddress.Parse("127.0.0.1"), 8889);
    }

    public void SendPositionUpdate(string position)
    {
        byte[] data = Encoding.ASCII.GetBytes(position);
        udpClient.Send(data, data.Length);
    }
}
