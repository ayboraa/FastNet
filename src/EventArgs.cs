namespace FastNet
{
    /// <summary>Contains event data for when a transport successfully establishes a connection to a client.</summary>
    public class ConnectedEventArgs
    {
        /// <summary>The newly established connection.</summary>
        public readonly Connection Connection;

        /// <summary>Initializes event data.</summary>
        /// <param name="connection">The newly established connection.</param>
        public ConnectedEventArgs(Connection connection)
        {
            Connection = connection;
        }


    }



    /// <summary>Contains event data for when a connection receives a message.</summary>
    public class MessageEventArgs
    {
        /// <summary>Received message.</summary>
        public readonly Message Message;


        /// <summary>Initializes event data.</summary>
        /// <param name="msg">The message that is received.</param>
        public MessageEventArgs(Message msg)
        {
            Message = msg;
        }


    }

    /// <summary>Contains event data for when a socket is disconnected.</summary>
    public class DisconnectEventArgs
    {
        public readonly string DisconnectReason;
        public readonly Connection Connection;

        public DisconnectEventArgs(string reason, Connection connection)
        {
            DisconnectReason = reason;
            Connection = connection;
        }


    }





}
