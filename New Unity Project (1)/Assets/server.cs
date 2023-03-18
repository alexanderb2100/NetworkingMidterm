using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Server
{
    class Program
    {
        static List<TcpClient> clients = new List<TcpClient>();

        static async Task Main(string[] args)
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 8080;

            TcpListener listener = new TcpListener(ipAddress, port);
            listener.Start();
            Console.WriteLine("Server started.");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                clients.Add(client);
                Console.WriteLine("Client connected.");

                // Handle client in separate task
                _ = Task.Run(() => HandleClient(client));
            }
        }

        static async Task HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            // Send initial position to client
            byte[] initialPosition = Encoding.ASCII.GetBytes("0,0");
            await stream.WriteAsync(initialPosition, 0, initialPosition.Length);

            while (true)
            {
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                // Relay message to all clients
                foreach (TcpClient c in clients)
                {
                    if (c != client)
                    {
                        NetworkStream clientStream = c.GetStream();
                        await clientStream.WriteAsync(buffer, 0, bytesRead);
                    }
                }

                Console.WriteLine($"Message from client: {message}");
            }
        }
    }
}