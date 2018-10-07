using System.IO;
using System.Net;
using Microsoft.Extensions.Logging;
using Ultz.SimpleServer.Internals;

namespace Ultz.Jfp.SimpleServer
{
    public class JfpConnection : IConnection
    {
        private IConnection _base;
        public JfpConnection(IConnection @base)
        {
            _base = @base;
            Pump = new JfpPump(Stream);
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