using System;
using System.IO;
using System.Text;
using Ultz.Jfp.SimpleServer;

namespace TestApp2
{
    public class TestHandlers
    {
        [HelloWorldTest]
        public void HelloWorldTest(JfpContext context)
        {
            var streamReader = new StreamReader(context.Stream, Encoding.UTF8);
            var streamWriter = new StreamWriter(context.Stream, Encoding.UTF8);
            var line = streamReader.ReadLine();
            Console.WriteLine(line);
            if (line == "Hello, world!")
            {
                streamWriter.WriteLine("Thou shalt not see the world!");
            }
            streamWriter.Close();
        }
    }
}