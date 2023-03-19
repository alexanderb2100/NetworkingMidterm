using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int tcpPort = 8888;
            int udpPort = 8889;

            TcpClient tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(ipAddress, tcpPort);
            Console.WriteLine("Connected to server.");

            UdpClient udpClient = new UdpClient();
            udpClient.Connect(ipAddress, udpPort);

            // Receive initial position from server
            NetworkStream stream = tcpClient.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string initialPosition = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Initial position: {initialPosition}");

            // Send position updates and chat messages to server
            while (true)
            {
                string input = Console.ReadLine();

                // Send position update to server
                if (input.Contains(","))
                {
                    byte[] positionUpdate = Encoding.ASCII.GetBytes(input);
                    udpClient.Send(positionUpdate, positionUpdate.Length);
                }

                // Send chat message to server
                else
                {
                    byte[] chatMessage = Encoding.ASCII.GetBytes($"[Client]: {input}");
                    await stream.WriteAsync(chatMessage, 0, chatMessage.Length);
                }
            }
        }
    }
}
