using FastNet;
using System;
using System.Net.Sockets;
using System.Threading;

namespace ServerExample
{
    public class Program
    {

        public static Server Instance;
        public static int BufferSize = 1024 * 1024;

        public static void Main() {

            Console.Title = "FastNet Server Example";

            const string address = "127.0.0.1";
            const int port = 7250;

            Server myServer;

            try
            {
                myServer = new Server(Transport.TCP, address, port, BufferSize);

                myServer.Start();


                Instance = myServer;

                new Thread(new ThreadStart(Loop)).Start();

                ChatServer c = new ChatServer();

            }
            catch (SocketException ex) {

                Console.WriteLine(ex.ToString());

            }

        }



        public static void Loop() {

            while (true) { 
            
                Instance.Update();

                Thread.Sleep(10);
            
            }
        
        
        }

    }
}
