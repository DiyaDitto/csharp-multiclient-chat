using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static readonly List<TcpClient> clients = new();
    static readonly Dictionary<TcpClient, string> usernames = new();

    static readonly object clientsLock = new();
    static readonly object usernamesLock = new();

    static void Main()
    {
        const int port = 6000;

        TcpListener server = new TcpListener(IPAddress.Any, port);
        server.Start();

        Console.WriteLine("================================");
        Console.WriteLine("      CHAT SERVER STARTED");
        Console.WriteLine("================================");
        Console.WriteLine($"Listening on port {port}...");
        Console.WriteLine("Waiting for clients...\n");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();

            lock (clientsLock)
            {
                clients.Add(client);
            }

            Console.WriteLine("Client connected!");

            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.Start();
        }
    }

    static void HandleClient(TcpClient client)
    {
        try
        {
            NetworkStream stream = client.GetStream();

            // First message from client is the username
            string username = ReadMessage(stream);

            if (string.IsNullOrWhiteSpace(username))
            {
                RemoveClient(client);
                return;
            }

            lock (usernamesLock)
            {
                usernames[client] = username;
            }

            Console.WriteLine($"{username} joined the chat.");

            Broadcast($"[SYSTEM] {username} joined the chat.");

            while (true)
            {
                string message = ReadMessage(stream);

                if (message == null)
                {
                    break;
                }

                if (string.IsNullOrWhiteSpace(message))
                {
                    continue;
                }

                string formattedMessage = $"{username}: {message}";

                Console.WriteLine(formattedMessage);

                Broadcast(formattedMessage);
            }
        }
        catch
        {
            // Client disconnected unexpectedly
        }
        finally
        {
            RemoveClient(client);
        }
    }

    static string ReadMessage(NetworkStream stream)
    {
        List<byte> data = new();

        while (true)
        {
            int value = stream.ReadByte();

            if (value == -1)
            {
                return data.Count == 0
                    ? null
                    : Encoding.UTF8.GetString(data.ToArray());
            }

            if (value == '\n')
            {
                break;
            }

            data.Add((byte)value);
        }

        return Encoding.UTF8.GetString(data.ToArray()).Trim();
    }

    static void SendMessage(TcpClient client, string message)
    {
        try
        {
            NetworkStream stream = client.GetStream();

            byte[] data = Encoding.UTF8.GetBytes(message + "\n");

            stream.Write(data, 0, data.Length);
        }
        catch
        {
            // Client may have disconnected
        }
    }

    static void Broadcast(string message)
    {
        List<TcpClient> connectedClients;

        lock (clientsLock)
        {
            connectedClients = new List<TcpClient>(clients);
        }

        foreach (TcpClient client in connectedClients)
        {
            SendMessage(client, message);
        }
    }

    static void RemoveClient(TcpClient client)
    {
        string username = null;

        lock (usernamesLock)
        {
            if (usernames.ContainsKey(client))
            {
                username = usernames[client];
                usernames.Remove(client);
            }
        }

        bool removed;

        lock (clientsLock)
        {
            removed = clients.Remove(client);
        }

        try
        {
            client.Close();
        }
        catch
        {
        }

        if (removed && username != null)
        {
            Console.WriteLine($"{username} left the chat.");

            Broadcast($"[SYSTEM] {username} left the chat.");
        }
    }
}
