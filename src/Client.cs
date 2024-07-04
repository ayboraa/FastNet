using FastNet.Tcp;
using System;
using System.Runtime.Intrinsics.X86;

namespace FastNet
{

    /// <summary>
    /// Provides a client to create a connection and handle packets within the server.
    /// </summary>
    public class Client
    {

        TcpClient tcpClient;
        TcpConnection connection;
        private Transport _transport;

        public EventHandler<ConnectedEventArgs> Connected;
        public EventHandler<MessageEventArgs> MessageReceived;



        /// <param name="transport">Transport to create client on.</param>
        public Client(Transport transport)
        {

            _transport = transport;

            if (transport == Transport.TCP)
            {
                tcpClient = new TcpClient(this);
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

            return tcpClient.Connect(address, port, out connection, out errorReason);

        }

        /// <summary>
        /// Disconnects from host.
        /// </summary>
        public void Disconnect() {

            if(connection.Socket.Connected)
                tcpClient.Close();

        }

        /// <summary>
        /// Sends a <see cref="Message"/> to the client.
        /// </summary>
        /// <param name="msg">Message to send.</param>
        public void Send(Message msg) {
        
               if(_transport == Transport.TCP)
                    connection.Send(msg);
        }

        /// <summary>
        /// Polls incoming messages from server.
        /// </summary>
        public void Poll() {
        
               if(_transport == Transport.TCP)
                    tcpClient.Poll();
        }



    }
}
