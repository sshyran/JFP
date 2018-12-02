using System.IO;
using System.Net;
using Ultz.SimpleServer.Internals;

namespace Ultz.Jfp.SimpleServer
{
    public class ConnectionClient : IJfpClient, IConnection
    {
        private IConnection _base;
        public ConnectionClient(IConnection connection)
        {
            _base = connection;
        }
        
        void IConnection.Close()
        {
            _base.Close();
        }

        public Stream Stream => _base.Stream;
        public bool Connected => _base.Connected;
        public EndPoint LocalEndPoint => _base.LocalEndPoint;
        public EndPoint RemoteEndPoint => _base.RemoteEndPoint;
        public int Id => _base.Id;

        Stream IJfpClient.Stream => Stream;

        bool IJfpClient.Connected => Connected;

        EndPoint IJfpClient.LocalEndPoint => LocalEndPoint;

        EndPoint IJfpClient.RemoteEndPoint => RemoteEndPoint;

        int IJfpClient.Id => Id;

        void IJfpClient.Close()
        {
            _base.Close();
        }
    }
}