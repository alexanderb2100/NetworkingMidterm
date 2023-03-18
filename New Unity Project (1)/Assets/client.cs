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
            int port = 8080;

            TcpClient client = new TcpClient();
            await client.ConnectAsync(ipAddress, port);
            Console.WriteLine("Connected to server.");

            // Receive initial position from server
            NetworkStream stream = client.GetStream();
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
                    await stream.WriteAsync(positionUpdate, 0, positionUpdate.Length);
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