using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Ultz.Jfp
{
    internal class TcpJfpClient : IJfpClient
    {
        private TcpClient _client;
        public TcpJfpClient(TcpClient client, int id)
        {
            _client = client;
            Id = id;
        }

        public Stream Stream => _client.GetStream();
        public bool Connected => _client.Connected;
        public EndPoint LocalEndPoint => _client.Client.LocalEndPoint;
        public EndPoint RemoteEndPoint => _client.Client.RemoteEndPoint;
        public int Id { get; }
        public void Close()
        {
            _client.Close();
        }
    }
}