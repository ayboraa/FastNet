using System;
using System.Net;
using System.Net.Sockets;

namespace FastNet.Udp
{


    /// <summary>
    /// A <see cref="Connection"/> that handles Udp transport.
    /// </summary>
    public class UdpConnection : Connection
    {

        public Socket Socket { get; private set; }

        public string? DisconnectReason {get; private set;}
        private bool isRunning;

        public event EventHandler<MessageEventArgs> MessageReceived;
        public event EventHandler<DisconnectEventArgs> Disconnected;

        public UdpConnection(Socket socket, IPEndPoint remoteEndPoint) : base(remoteEndPoint) {
        
            Socket = socket;
           
        }


        public void OpenSocket() {

            if (isRunning)
                return;

            isRunning = true;
        
        }

        public void CloseSocket () {

            if (!isRunning)
                return;

            isRunning = false;

            Socket.Close();
        
        }


        
        public void Send(Message msg) {

            if (!isRunning)
                return;

            Socket.SendTo(msg.GetBuffer(), RemoteEndPoint);
        
        }


    }
}
