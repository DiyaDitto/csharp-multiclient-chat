using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static void Main()
    {
        const string serverIp = "127.0.0.1";
        const int port = 6000;

        try
        {
            TcpClient client = new TcpClient();

            Console.WriteLine("Connecting to server...");

            client.Connect(serverIp, port);

            Console.WriteLine("================================");
            Console.WriteLine("       CONNECTED TO CHAT");
            Console.WriteLine("================================");

            NetworkStream stream = client.GetStream();

            // Username
            Console.Write("Enter your name: ");

            string username = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(username))
            {
                Console.Write("Name cannot be empty. Enter your name: ");
                username = Console.ReadLine();
            }

            SendMessage(stream, username);

            Console.WriteLine();
            Console.WriteLine("You can now start chatting.");
            Console.WriteLine("Type /exit to leave.");
            Console.WriteLine();

            // Thread for receiving messages
            Thread receiveThread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        string message = ReadMessage(stream);

                        if (message == null)
                        {
                            Console.WriteLine("\nDisconnected from server.");
                            break;
                        }

                        Console.WriteLine(message);
                    }
                }
                catch
                {
                    Console.WriteLine("\nConnection closed.");
                }
            });

            receiveThread.Start();

            // Main thread sends messages
            while (true)
            {
                string message = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(message))
                {
                    continue;
                }

                if (message.Equals("/exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                SendMessage(stream, message);
            }

            client.Close();

            Console.WriteLine("You left the chat.");
        }
        catch (SocketException)
        {
            Console.WriteLine();
            Console.WriteLine("Could not connect to the server.");
            Console.WriteLine("Make sure the ChatServer is running.");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    static void SendMessage(NetworkStream stream, string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message + "\n");

        stream.Write(data, 0, data.Length);
    }

    static string ReadMessage(NetworkStream stream)
    {
        StringBuilder message = new();

        while (true)
        {
            int value = stream.ReadByte();

            if (value == -1)
            {
                return null;
            }

            if (value == '\n')
            {
                break;
            }

            message.Append((char)value);
        }

        return message.ToString();
    }
}
