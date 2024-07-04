using FastNet.Tcp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientExample
{
    public class ChatClient
    {

        public ChatClient()
        {
            Program.Instance.MessageReceived += OnMessageReceived;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

        }

        private void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
              Program.Instance.Disconnect();
        }

        private void OnMessageReceived(object? sender, MessageEventArgs args)
        {
            if (sender == null)
                return;

            TcpConnection connection = (TcpConnection)sender;
            Console.WriteLine(args.Message.ReadString());

        }



    }
}
