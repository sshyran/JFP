using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging.Console;
using Ultz.Jfp;
using Ultz.SimpleServer.Common;

namespace TestApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new TestService();
            service.Add(new Connector(IPAddress.Any,Jfp.Port));
            service.LoggerProvider = new ConsoleLoggerProvider((s, level) => true, false, false);
            service.Start();
            var pump = Jfp.Connect("jfp://localhost/");
            var stream = pump.Send("HELLO_WORLD_TEST", Encoding.UTF8.GetBytes("Hello, world!\n"));
            var streamReader = new StreamReader(stream);
            Thread.Sleep(1000);
            var line = streamReader.ReadLine();
            Console.WriteLine("Test Result: " + (line == "Thou shalt not see the world!") + " (" + line + ")");
            Console.ReadLine();
            service.Dispose();
            pump.Dispose();
        }
    }
}