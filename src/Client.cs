using FastNet.Tcp;
using FastNet.Udp;
using System;
using System.Reflection;
using System.Runtime.Intrinsics.X86;

namespace FastNet
{

    /// <summary>
    /// Provides a client to create a connection and handle packets within the server.
    /// </summary>
    public class Client
    {

        TcpClient tcpClient;
        TcpConnection tcpConnection;
        UdpClient udpClient;
        UdpConnection udpConnection;
        private Transport _transport;

        public EventHandler<ConnectedEventArgs> Connected;
        public EventHandler<MessageEventArgs> MessageReceived;



        /// <param name="transport">Transport to create client on.</param>
        public Client(Transport transport, int bufferSize = 1024)
        {

            _transport = transport;

            if (transport == Transport.TCP)
            {
                tcpClient = new TcpClient(this, bufferSize);
            }
            else
            {
                udpClient = new UdpClient(this, bufferSize);
            }

        }

        /// <summary>
        /// Tries to connect to a host.
        /// </summary>
        /// <param name="address">IP address of remote server.</param>
        /// <param name="port">Port address of remote server.</param>
        /// <returns> <see langword = "true" /> if successfully connected; <see langword = "false" /> if there was an error during the connection. </returns>
        public bool Connect(string address, Int32 port) {
            string errorReason;

            if(_transport == Transport.TCP) 
                return tcpClient.Connect(address, port, out tcpConnection, out errorReason);
            else
                return udpClient.Connect(address, port, out udpConnection, out errorReason);

        }

        /// <summary>
        /// Disconnects from host.
        /// </summary>
        public void Disconnect() {

            if (tcpConnection.Socket.Connected)
                tcpClient.Close();
            else if (udpConnection.Socket.Connected)
                udpClient.Close();
            else
                return;


        }

        /// <summary>
        /// Sends a <see cref="Message"/> to the client.
        /// </summary>
        /// <param name="msg">Message to send.</param>
        public void Send(Message msg) {

            if (_transport == Transport.TCP)
                tcpConnection.Send(msg);
            else
                udpConnection.Send(msg);
        }

        /// <summary>
        /// Polls incoming messages from server.
        /// </summary>
        public void Poll() {

            if (_transport == Transport.TCP)
                tcpClient.Poll();
            else
                udpClient.Poll();
        }



    }
}
