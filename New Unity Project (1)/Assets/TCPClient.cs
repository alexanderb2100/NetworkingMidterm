using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class TCPClient : MonoBehaviour
{
    private TcpClient tcpClient;
    private Thread receiveThread;
    private NetworkStream stream;

    public void Start()
    {
        tcpClient = new TcpClient();
        tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 8888);
        stream = tcpClient.GetStream();

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.Start();
    }

    public void SendChatMessage(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes($"[Client]: {message}");
        stream.Write(data, 0, data.Length);
    }

    private void ReceiveData()
    {
        while (true)
        {
            byte[] data = new byte[1024];
            int bytesRead = stream.Read(data, 0, data.Length);
            string message = Encoding.ASCII.GetString(data, 0, bytesRead);

            Debug.Log(message);
        }
    }
}
