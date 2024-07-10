using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace FastNet.Udp
{


    /// <summary>
    /// A server that interacts with <see cref="UdpClient"/>.
    /// </summary>
    public class UdpServer
    {

        public Int32 Port { get; private set; }
        public readonly int SocketBufferSize;

        private Socket socket;
        private bool isRunning = false;
        protected const int DefaultSocketBufferSize = 1024;
        private string _listenAddress;
        private Dictionary<IPEndPoint, UdpConnection> connections;


        private int _maxPendingConnections = 5;
        private Dictionary<IPEndPoint, UdpConnection> closedConnections;

        private Server mainServer;

        public UdpServer(Server s, string address, Int32 port, int socketBufferSize = DefaultSocketBufferSize, int maxPendingConnections = 5) {

            mainServer = s;
            Port = port;
            _listenAddress = address;
            _maxPendingConnections = maxPendingConnections;
            SocketBufferSize = socketBufferSize;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            

        }

        public void Start() {

            connections = new Dictionary<IPEndPoint, UdpConnection>();
            closedConnections = new Dictionary<IPEndPoint, UdpConnection>();

            startListening();

        }

        private void startListening() {

            stopListening();

            if (isRunning)
                return;


            socket.ReceiveBufferSize = SocketBufferSize;
            socket.SendBufferSize = SocketBufferSize;


            isRunning = true;

        }



        protected void HandleConnection(IPEndPoint iPEndPoint, Message msg) {

            if (!isRunning)
                return;

            UdpConnection conn = new UdpConnection(socket, iPEndPoint);
            connections.Add(iPEndPoint, conn);
            conn.OpenSocket();
            mainServer.Connected?.Invoke(conn, new ConnectedEventArgs(conn));
            conn.MessageReceived += OnMessageReceived;
            conn.Disconnected += OnDisconnected;

        }

        protected virtual void OnDisconnected(object? sender, DisconnectEventArgs args) {

            UdpConnection conn = (UdpConnection)sender;

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

            receive();
            
            foreach (UdpConnection connection in closedConnections.Values) {
                mainServer.Disconnected.Invoke(connection, new DisconnectEventArgs(connection.DisconnectReason, connection));
                connections.Remove(connection.RemoteEndPoint);
                closedConnections.Remove(connection.RemoteEndPoint);
                connection.CloseSocket();
            }

        }

        private void receive() {
        
            if(socket.Available >= 1)
            {
                EndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                byte[] buffer = new Byte[this.SocketBufferSize];
                if(socket.ReceiveFrom(buffer, buffer.Length, SocketFlags.None, ref sender) >= 1)
                {
                    if (!connections.ContainsKey((IPEndPoint)sender))
                        HandleConnection((IPEndPoint)sender, new Message(buffer.Length, buffer));
                    else
                        mainServer.MessageReceived.Invoke(connections[(IPEndPoint)sender], new MessageEventArgs(new Message(buffer.Length, buffer)));   


                }

            }
        
        }        


        private void stopListening() {

            if (!isRunning)
                return;

            isRunning = false;
            socket.Shutdown(SocketShutdown.Receive);   

        }





    }
}
