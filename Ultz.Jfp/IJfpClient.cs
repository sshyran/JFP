using System.IO;
using System.Net;

namespace Ultz.Jfp
{
    public interface IJfpClient
    {
        Stream Stream { get; }
        bool Connected { get; }
        EndPoint LocalEndPoint { get; }
        EndPoint RemoteEndPoint { get; }
        int Id { get; }
        void Close();
    }
}