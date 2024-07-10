using FastNet;
using FastNet.Tcp;
using System;

namespace ServerExample
{
    public class ChatServer
    {


        public ChatServer()
        {
            Program.Instance.Connected += OnClientConnected;
            Program.Instance.MessageReceived += OnMessageReceived;
            Program.Instance.Disconnected += OnClientDisconnected;
        }

        private void OnClientDisconnected(object? sender, DisconnectEventArgs args) {

            if (sender == null)
                return;

            TcpConnection connection = (TcpConnection)sender;
            Console.WriteLine($"Client disconnected: {connection.Socket.RemoteEndPoint}");
        
        }
        private void OnClientConnected(object? sender, ConnectedEventArgs args) {

            if (sender == null)
                return;

            TcpConnection connection = (TcpConnection)sender;
            Console.WriteLine($"Client connected: {connection.Socket.RemoteEndPoint}");
        
        }

        
        private void OnMessageReceived(object? sender, MessageEventArgs args)
        {
            if (sender == null)
                return;

            TcpConnection connection = (TcpConnection)sender;

            string userMessage = args.Message.ReadString();
            Console.WriteLine($"[{connection.Socket.RemoteEndPoint}]: {userMessage}");
            Message newMsg = new Message(Program.BufferSize);
            newMsg.Write($"[{connection.Socket.RemoteEndPoint}]: {userMessage}");
            Program.Instance.Broadcast(newMsg);

        }


    }
}
