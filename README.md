C# Multi-Client Chat Application

A real-time, console-based multi-client chat application built using C# and .NET TCP sockets.

The application follows a client-server architecture, where multiple clients connect to a central server and exchange messages in real time.

🚀 Features
Multi-client communication
Real-time messaging
Username support
Message broadcasting
Join/leave notifications
Multithreaded client handling
Thread-safe client management
Graceful client disconnection
/exit command
UTF-8 message support
🛠️ Technologies Used
C#
.NET
TCP/IP
TcpListener
TcpClient
NetworkStream
Multithreading
Thread synchronization using lock




The server maintains a list of connected clients. When a client sends a message, the server broadcasts it to all connected clients.

📁 Project Structure

csharp-multiclient-chat/

├── ChatServer/
   ├── Program.cs
   └── ChatServer.csproj

├── ChatClient/
   ├── Program.cs
   └── ChatClient.csproj

├── .gitignore
└── README.md

⚙️ Requirements

Before running the application, make sure you have:

.NET SDK installed
Linux, Windows, or macOS
Terminal

Check your .NET installation:

dotnet --version
▶️ How to Run
1. Clone the repository
git clone <your-repository-url>
cd csharp-multiclient-chat
2. Start the server

Open Terminal 1:

cd ChatServer
dotnet run

Expected output:

================================
      CHAT SERVER STARTED
================================
Listening on port 6000...
Waiting for clients...
3. Start Client 1

Open Terminal 2:

cd ChatClient
dotnet run

Enter a username:

Enter your name: Diya
4. Start Client 2

Open Terminal 3:

cd ChatClient
dotnet run

Enter another username:

Enter your name: Rahul

You can open additional terminals to connect more clients.

💬 Example

Client 1:

Enter your name: Diya

Diya: Hello Rahul

Client 2:

Enter your name: Rahul

[SYSTEM] Diya joined the chat.
Diya: Hello Rahul

Rahul: Hi Diya

When Rahul exits:

[SYSTEM] Rahul left the chat.

To leave the chat:

/exit
🧠 Key Concepts
Client-Server Architecture

The server listens for incoming connections while clients connect to the server.

TCP Sockets

TcpListener is used by the server to listen for connections, while TcpClient is used by clients to establish TCP connections.

NetworkStream

NetworkStream provides the communication channel through which clients and the server send and receive data.

Multithreading

Each connected client is handled using a separate thread, allowing multiple clients to communicate simultaneously.

Thread Safety

The server uses lock to safely access shared collections when multiple client threads are running concurrently.

Message Framing

TCP is stream-based, so the application uses newline-delimited messages to determine where each message ends.

🔄 Message Flow
Client
   │
   │ "Hello"
   ▼
Chat Server
   │
   │ Broadcast
   ├──────────────► Client 1
   ├──────────────► Client 2
   └──────────────► Client 3
