using FastNet.Tcp;
using FastNet.Udp;
using System;
using System.Net;

namespace FastNet
{


    /// <summary>
    /// Provides a server to communicate with <see cref="Client"/>
    /// </summary>
    public class Server
    {

        TcpServer tcpServer;
        UdpServer udpServer;
        private Transport _transport;


        public EventHandler<ConnectedEventArgs>? Connected;
        public EventHandler<MessageEventArgs>? MessageReceived;
        public EventHandler<DisconnectEventArgs>? Disconnected;


        /// <param name="transport">Transport to decide protocol.</param>
        /// <param name="address">IP address to listen on.</param>
        /// <param name="port">Port number to listen on.</param>
        public Server(Transport transport, string address, int port, int bufferSize = 1024, int maxPendingConnections = 5) {

            _transport = transport;

            if (transport == Transport.TCP)
            {
                tcpServer = new TcpServer(this, address, port, bufferSize, maxPendingConnections);
            }
            else
            {
                udpServer = new UdpServer(this, address, port, bufferSize, maxPendingConnections);
            }
        
        }




        /// <summary>
        /// Broadcasts the given message among connected clients.
        /// </summary>
        /// <param name="msg">Message to broadcast.</param>
        public void Broadcast(Message msg) {

            if(_transport == Transport.TCP)
                tcpServer.Broadcast(msg);
            else
                udpServer.Broadcast(msg);

        }


        /// <summary>
        /// Closes a connection with given <see cref="IPEndPoint"/>
        /// </summary>
        public void CloseConnection(IPEndPoint iPEndPoint) {

            if (_transport == Transport.TCP)
                tcpServer.CloseConnection(iPEndPoint);
            else
                udpServer.CloseConnection(iPEndPoint);



        }
        /// <summary>
        /// Starts server so it can listen incoming connections.
        /// </summary>
        public void Start() {

            if(_transport == Transport.TCP)
                tcpServer.Start();
            else
                udpServer.Start();

            Update();

        }


        /// <summary>
        /// Accepts incoming request, closes connections and receive new packets.
        /// </summary>
        public void Update() {

            if (_transport == Transport.TCP)
                tcpServer.Poll();
            else
                udpServer.Poll();

        }


    }
}
