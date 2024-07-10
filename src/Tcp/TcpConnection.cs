using System;
using System.Net;
using System.Net.Sockets;

namespace FastNet.Tcp
{


    /// <summary>
    /// A <see cref="Connection"/> that handles Tcp transport.
    /// </summary>
    public class TcpConnection : Connection
    {

        public Socket Socket { get; private set; }
        public System.Net.Sockets.TcpClient Client { get; private set; }

        public IPEndPoint RemoteEndpoint { get; private set; }
        public string? DisconnectReason {get; private set;}
        private bool isRunning;

        public event EventHandler<MessageEventArgs> MessageReceived;
        public event EventHandler<DisconnectEventArgs> Disconnected;

        public TcpConnection(System.Net.Sockets.TcpClient client, Socket socket, IPEndPoint remoteEndPoint) : base(remoteEndPoint) {
        
            Socket = socket;
            Client = client;
        }


        public void OpenSocket() {

            if (isRunning)
                return;

            Socket = Client.Client;
            isRunning = true;
            RemoteEndpoint = (IPEndPoint)Socket.RemoteEndPoint;
        
        }

        public void CloseSocket () {

            if (!isRunning)
                return;

            isRunning = false;

            Client.Close();
        
        }



        public void Receive() {

            if (!isRunning)
                return;


            byte[] buffer = new byte[Client.ReceiveBufferSize];
            if(Client.Available >= 1 && Client.GetStream().Read(buffer, 0, buffer.Length) != 0) {

                using (Message msg = new Message(buffer.Length, buffer)) {

                    MessageReceived?.Invoke(this, new MessageEventArgs(msg));

                }
            }
            else
            {
                if (Socket.Poll(1000, SelectMode.SelectRead) && Client.Available == 0)
                {
                    // Socket disconnected.
                    DisconnectReason = "Socket disconnected.";
                    Disconnected?.Invoke(this, new DisconnectEventArgs(DisconnectReason, this));

                }


            }

        }
        
        public void Send(Message msg) {

            if (!isRunning)
                return;

            Client.GetStream().Write(msg.GetBuffer(), 0, msg.GetBuffer().Length);
        
        }


    }
}
