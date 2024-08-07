﻿using FastNet;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace ClientExample
{
    public class Program
    {
        public static Client Instance;
        public static int BufferSize = 1024 * 1024;

        public static void Main() {

            const string address = "127.0.0.1";
            const int port = 7250;
            

            Console.Title = "FastNet Chat Client Example";


            Instance = new Client(Transport.TCP, BufferSize);

            do
            {

                Console.WriteLine($"Trying to connect to the host {address}:{port}...");

                Thread.Sleep(2500);


            } while (!Instance.Connect(address, port));

            Console.WriteLine("Connected.");

            ChatClient c = new ChatClient();

            new Thread(new ThreadStart(Loop)).Start();


            while (true) { 
            
                string? msg = Console.ReadLine();
                if (msg != null)
                {

                    Util.DeletePrevConsoleLine();
                    Message message = new Message(BufferSize);
                    message.Write(msg);

                    Instance.Send(message);

                }

            }
            

        }




        public static void Loop()
        {

            while (true)
            {

                Instance.Poll();

                Thread.Sleep(10);

            }


        }


    }
}
