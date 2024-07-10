using System.Net;

namespace FastNet
{


    /// <summary>
    /// Defines the protocols to be used during transportation.
    /// </summary>
    public enum Transport
    {
        TCP,
        UDP
    }


    /// <summary>
    /// The connection is used to interact with the socket directly, such as for sending or receiving packets.
    /// </summary>
    public abstract class Connection
    {

        public IPEndPoint RemoteEndPoint { get; private set; }

        /// <param name="remoteEndPoint">IPEndPoint to recognize the connection.</param>
        public Connection(IPEndPoint remoteEndPoint)
        {
            RemoteEndPoint = remoteEndPoint;
        }
    }
}
