using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ultz.Jfp;
using Ultz.Jfp.IO;

namespace TestApp1
{
    class Program
    {
        private static JfpListener _listener;

        static void Main(string[] args)
        {
            _listener = new JfpListener(new TcpListener(IPAddress.Any, Jfp.Port));
            Task.Run(() => { ServerRunner(); });
            Thread.Sleep(1000);
            var pump = Jfp.Connect("jfp://localhost/");
            var stream = pump.Send("HELLO_WORLD_TEST", Encoding.UTF8.GetBytes("Hello, world!\n"));
            var streamReader = new StreamReader(stream);
            Thread.Sleep(1000);
            var line = streamReader.ReadLine();
            Console.WriteLine("Test Result: " + (line == "Thou shalt not see the world!") + " (" + line + ")");
            Console.ReadLine();
        }

        static void ServerRunner()
        {
            _listener.Start();
            while (true)
            {
                GotPump(_listener.AcceptJfpPump());
            }
        }

        static void GotPump(JfpPump pump)
        {
            Console.WriteLine("acalled");
            pump.OnCommand += PumpOnOnCommand;
        }

        private static void PumpOnOnCommand(object sender, OnCommandEventArgs e)
        {
            Console.WriteLine("called");
            if (e.MessageType == "HELLO_WORLD_TEST")
            {
                var streamReader = new StreamReader(e.DataStream, Encoding.UTF8);
                var streamWriter = new StreamWriter(e.DataStream, Encoding.UTF8);
                var line = streamReader.ReadLine();
                Console.WriteLine(line);
                if (line == "Hello, world!")
                {
                    streamWriter.WriteLine("Thou shalt not see the world!");
                    streamWriter.Close();
                }
            }
            else
            {
                e.DataStream.Close();
            }
        }
    }
}