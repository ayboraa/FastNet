using System.Net.Sockets;
using System.Net;
using System;

namespace FastNet.Udp
{

    /// <summary>
    /// A client that interacts with <see cref="UdpServer"/>.
    /// </summary>
    public class UdpClient
    {

        private UdpConnection _connection;
        private Socket socket;
        protected const int DefaultSocketBufferSize = 1024;
        public readonly int SocketBufferSize;

        private Client mainClient;


        /// <param name="c">Base client to invoke it's events.</param>
        public UdpClient(Client c, int bufferSize = DefaultSocketBufferSize) {

            SocketBufferSize = bufferSize;
            mainClient = c;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        
        }
        
        public bool Connect(string address, Int32 port, out UdpConnection? connection, out string connectError)
        {
            connectError = "null";

            IPAddress remoteAddr = IPAddress.Parse(address);
            IPEndPoint remoteEndPoint = new IPEndPoint(remoteAddr, port);



            try { 
                socket.Connect(remoteEndPoint);
                connection = _connection = new UdpConnection(socket, remoteEndPoint);
                _connection.OpenSocket();
                Message connectedMessage = new Message();
                connectedMessage.Write(address);
                _connection.Send(connectedMessage);
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
            socket.Dispose();

        }

        public void Poll()
        {
            if (!socket.Connected)
                return;

            receive();



        }

        private void receive()
        {

            if (socket.Available >= 1)
            {
                EndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                byte[] buffer = new Byte[this.SocketBufferSize];
                if (socket.ReceiveFrom(buffer, buffer.Length, SocketFlags.None, ref sender) >= 1)
                {
                    OnMessageReceived(_connection, new MessageEventArgs(new Message(buffer.Length, buffer)));   
                }

            }

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
