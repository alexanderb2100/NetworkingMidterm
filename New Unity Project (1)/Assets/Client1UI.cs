using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class client1UI : MonoBehaviour
{
    // Server IP address and port
    public string serverIP = "127.0.0.1";
    public int serverPort = 8888;

    // UDP client for position updates
    private UdpClient udpClient;

    // TCP client for chat messages
    private TcpClient tcpClient;
    private NetworkStream tcpStream;

    // Start is called before the first frame update
    void Start()
    {
        // Connect to UDP server for position updates
        udpClient = new UdpClient();
        udpClient.Connect(serverIP, serverPort + 1);

        // Connect to TCP server for chat messages
        tcpClient = new TcpClient();
        tcpClient.Connect(serverIP, serverPort);
        tcpStream = tcpClient.GetStream();

        // Send initial position to server
        string initialPosition = "1,0,0,0,0,0";
        byte[] initialPositionBytes = Encoding.ASCII.GetBytes(initialPosition);
        udpClient.Send(initialPositionBytes, initialPositionBytes.Length);

        // Start receiving chat messages in separate thread
        byte[] buffer = new byte[1024];
        tcpStream.BeginRead(buffer, 0, buffer.Length, OnChatMessageReceived, buffer);
    }

    // Update is called once per frame
    void Update()
    {
        // Get position and rotation data
        string position = transform.position.x.ToString("F2") + "," + transform.position.y.ToString("F2") + "," + transform.position.z.ToString("F2");
        string rotation = transform.rotation.eulerAngles.x.ToString("F2") + "," + transform.rotation.eulerAngles.y.ToString("F2") + "," + transform.rotation.eulerAngles.z.ToString("F2");

        // Send position and rotation data to server
        string positionUpdate = "1," + position + "," + rotation;
        byte[] positionUpdateBytes = Encoding.ASCII.GetBytes(positionUpdate);
        udpClient.Send(positionUpdateBytes, positionUpdateBytes.Length);

        // Send chat message to server
        if (Input.GetKeyDown(KeyCode.Return))
        {
            string chatMessage = "[Client 1]: " + GetComponent<UnityEngine.UI.InputField>().text;
            GetComponent<UnityEngine.UI.InputField>().text = "";
            byte[] chatMessageBytes = Encoding.ASCII.GetBytes(chatMessage);
            tcpStream.Write(chatMessageBytes, 0, chatMessageBytes.Length);
        }
    }

    // Callback for receiving chat messages
    private void OnChatMessageReceived(IAsyncResult result)
    {
        // Get chat message and display it on UI
        byte[] buffer = (byte[])result.AsyncState;
        int bytesRead = tcpStream.EndRead(result);
        string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        GetComponentInChildren<UnityEngine.UI.Text>().text += message + "\n";

        // Start receiving next chat message
        tcpStream.BeginRead(buffer, 0, buffer.Length, OnChatMessageReceived, buffer);
    }

    // Clean up resources on exit
    private void OnApplicationQuit()
    {
        udpClient.Close();
        tcpStream.Close();
        tcpClient.Close();
    }
}
