using System.Net.Sockets;
using System.Net;
using System;

namespace FastNet.Tcp
{

    /// <summary>
    /// A client that interacts with <see cref="TcpServer"/>.
    /// </summary>
    public class TcpClient
    {

        private System.Net.Sockets.TcpClient _tcpClient;
        private TcpConnection _connection;
        private Socket socket;
        protected const int DefaultSocketBufferSize = 1024 * 1024;
        public readonly int SocketBufferSize;

        private Client mainClient;


        /// <param name="c">Base client to invoke it's events.</param>
        public TcpClient(Client c, int bufferSize = DefaultSocketBufferSize) {

            SocketBufferSize = bufferSize;
            mainClient = c;
        
        }
        
        public bool Connect(string address, Int32 port, out TcpConnection? connection, out string connectError)
        {
            connectError = "null";

            IPAddress remoteAddr = IPAddress.Parse(address);
            IPEndPoint remoteEndPoint = new IPEndPoint(remoteAddr, port);


            _tcpClient = new System.Net.Sockets.TcpClient();
            _tcpClient.SendBufferSize = SocketBufferSize;
            _tcpClient.ReceiveBufferSize = SocketBufferSize;

            try { 
                _tcpClient.Connect(remoteEndPoint);
                connection = _connection = new TcpConnection(_tcpClient, socket, remoteEndPoint);
                _connection.OpenSocket();
                mainClient.Connected?.Invoke(this, new ConnectedEventArgs(connection));
                connection.MessageReceived += OnMessageReceived;
                connection.Disconnected += OnSocketDisconnected;

                return true;
            }
            catch(SocketException ex)
            {
                connection = null;

                return false;
            }


            
        }


        public void Close()
        {
            _connection.CloseSocket();
            _tcpClient.Dispose();

        }

        public void Poll()
        {
            if (!_tcpClient.Connected)
                return;

            _connection.Receive();



        }

        protected virtual void OnMessageReceived(object? sender, MessageEventArgs args)
        {
            mainClient.MessageReceived?.Invoke(sender, args);

        }
        
        protected virtual void OnSocketDisconnected(object? sender, DisconnectEventArgs args)
        {
            Close();
        }


    }
}
