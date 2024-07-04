using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace FastNet.Tcp
{


    /// <summary>
    /// A server that interacts with <see cref="TcpClient"/>.
    /// </summary>
    public class TcpServer
    {

        public Int32 Port { get; private set; }
        public readonly int SocketBufferSize;

        private TcpListener listener;
        private bool isRunning = false;
        protected const int DefaultSocketBufferSize = 1024 * 1024;
        private string _listenAddress;
        private Dictionary<IPEndPoint, TcpConnection> connections;


        private int _maxPendingConnections = 5;
        private Dictionary<IPEndPoint, TcpConnection> closedConnections;

        private Server mainServer;

        public TcpServer(Server s, string address, Int32 port, int socketBufferSize = DefaultSocketBufferSize, int maxPendingConnections = 5) {

            mainServer = s;
            Port = port;
            _listenAddress = address;
            _maxPendingConnections = maxPendingConnections;
            SocketBufferSize = socketBufferSize;
            

        }

        public void Start() {

            connections = new Dictionary<IPEndPoint, TcpConnection>();
            closedConnections = new Dictionary<IPEndPoint, TcpConnection>();

            startListening();

        }

        private void startListening() {

            stopListening();

            if (isRunning)
                return;


            IPAddress localAddr = IPAddress.Parse(_listenAddress);
            listener = new TcpListener(localAddr, Port);
            listener.Server.ReceiveBufferSize = SocketBufferSize;
            listener.Server.SendBufferSize = SocketBufferSize;
            listener.Start(_maxPendingConnections);

            isRunning = true;

        }



        protected void Accept() {

            if (!isRunning)
                return;

            // Do not block if there's no connection.
            if (!listener.Pending())
                return;

            System.Net.Sockets.TcpClient newClient = listener.AcceptTcpClient();
            TcpConnection conn = new TcpConnection(newClient, newClient.Client, (IPEndPoint)newClient.Client.RemoteEndPoint);
            connections.Add((IPEndPoint)newClient.Client.RemoteEndPoint, conn);
            conn.OpenSocket();
            mainServer.Connected?.Invoke(conn, new ConnectedEventArgs(conn));
            conn.MessageReceived += OnMessageReceived;
            conn.Disconnected += OnDisconnected;

        }

        protected virtual void OnDisconnected(object? sender, DisconnectEventArgs args) {

            TcpConnection conn = (TcpConnection)sender;

            CloseConnection(conn.RemoteEndPoint);

        }
        protected virtual void OnMessageReceived(object? sender, MessageEventArgs args) {

            mainServer.MessageReceived?.Invoke(sender, args);

        }

        public void CloseConnection(IPEndPoint remoteEndPoint)
        {
            if (connections.ContainsKey(remoteEndPoint) && !closedConnections.ContainsKey(remoteEndPoint))
                closedConnections.Add(remoteEndPoint, connections[remoteEndPoint]);

        }


        public void Broadcast(Message msg)
        {

            foreach (var item in connections)
            {
                item.Value.Send(msg);
            }
        }



        public void Poll() {

            if (!isRunning)
                return;

            Accept();

            foreach (TcpConnection connection in connections.Values)
                connection.Receive();
            
            foreach (TcpConnection connection in closedConnections.Values) {
                mainServer.Disconnected.Invoke(connection, new DisconnectEventArgs(connection.DisconnectReason, connection));
                connections.Remove(connection.RemoteEndPoint);
                closedConnections.Remove(connection.RemoteEndPoint);
                connection.CloseSocket();
            }




        }

        private void stopListening() {

            if (!isRunning)
                return;

            isRunning = false;
            this.listener.Stop();   

        }





    }
}
