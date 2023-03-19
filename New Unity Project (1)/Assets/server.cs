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
        static List<UdpClient> udpClients = new List<UdpClient>();
        static List<TcpClient> tcpClients = new List<TcpClient>();

        static async Task Main(string[] args)
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int tcpPort = 8888;
            int udpPort = 8889;

            TcpListener listener = new TcpListener(ipAddress, tcpPort);
            listener.Start();
            Console.WriteLine($"Server started. Listening on TCP port {tcpPort} and UDP port {udpPort}");

            // Listen for UDP packets in a separate thread
            _ = Task.Run(() => ListenForUdpPackets(ipAddress, udpPort));

            while (true)
            {
                TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                tcpClients.Add(tcpClient);
                Console.WriteLine("TCP client connected.");

                // Handle TCP client in separate task
                _ = Task.Run(() => HandleTcpClient(tcpClient));
            }
        }

        static async Task HandleTcpClient(TcpClient tcpClient)
        {
            NetworkStream stream = tcpClient.GetStream();

            // Send initial position to client
            byte[] initialPosition = Encoding.ASCII.GetBytes("0,0");
            await stream.WriteAsync(initialPosition, 0, initialPosition.Length);

            while (true)
            {
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                // Relay message to all TCP clients
                foreach (TcpClient client in tcpClients)
                {
                    if (client != tcpClient)
                    {
                        NetworkStream clientStream = client.GetStream();
                        await clientStream.WriteAsync(buffer, 0, bytesRead);
                    }
                }

                Console.WriteLine($"TCP message from client: {message}");
            }
        }

        static async Task ListenForUdpPackets(IPAddress ipAddress, int udpPort)
        {
            UdpClient udpListener = new UdpClient(udpPort);

            while (true)
            {
                // Receive UDP packet
                UdpReceiveResult result = await udpListener.ReceiveAsync();
                byte[] receivedBytes = result.Buffer;
                string message = Encoding.ASCII.GetString(receivedBytes);

                // Relay message to all UDP clients
                foreach (UdpClient udpClient in udpClients)
                {
                    // Don't send the message back to the sender
                    if (!result.RemoteEndPoint.Equals(udpClient.Client.LocalEndPoint))
                    {
                        await udpClient.SendAsync(receivedBytes, receivedBytes.Length, result.RemoteEndPoint);
                    }
                }

                Console.WriteLine($"UDP message from client: {message}");
            }
        }
    }
}
