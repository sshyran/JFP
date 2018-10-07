using System;
using Ultz.Jfp.SimpleServer;

namespace TestApp2
{
    public class HelloWorldTestAttribute : JfpAttribute
    {
        public override string MessageType => "HELLO_WORLD_TEST";
    }
}