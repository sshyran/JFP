using System;

namespace Ultz.Jfp.SimpleServer
{
    public abstract class JfpAttribute : Attribute
    {
        public abstract string MessageType { get; }
    }
}