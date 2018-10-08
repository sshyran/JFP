using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Ultz.SimpleServer.Internals;

namespace Ultz.Jfp.SimpleServer
{
    public class JfpConnection : IConnection
    {
        private IConnection _base;

        public JfpConnection(IConnection @base, JfpPump pump)
        {
            _base = @base;
            Pump = pump;
        }
        
        public void Close()
        {
            Pump.Dispose();
            _base.Close();
        }

        public Stream Stream => _base.Stream;
        public bool Connected => _base.Connected;
        public EndPoint LocalEndPoint => _base.LocalEndPoint;
        public EndPoint RemoteEndPoint => _base.RemoteEndPoint;
        public int Id => _base.Id;
        public JfpPump Pump { get; }
    }
}