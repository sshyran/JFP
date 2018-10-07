using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Logging;
using Ultz.Jfp.IO;
using Ultz.SimpleServer.Internals;

namespace Ultz.Jfp.SimpleServer
{
    public class JfpContext : IContext, IRequest, IResponse, IMethod
    {
        public JfpContext(JfpConnection connection, ILogger logger, JfpStream stream)
        {
            Connection = connection;
            Logger = logger;
            Id = stream.Id;
            MessageType = stream.MessageType;
            Stream = stream;
            Pump = connection.Pump;
        }
        #region Explicit Members

        IRequest IContext.Request => this;
        IResponse IContext.Response => this;
        IMethod IRequest.Method => this;
        byte[] IMethod.Id => Encoding.UTF8.GetBytes(MessageType);
        Stream IRequest.InputStream => Stream;
        Stream IResponse.OutputStream => Stream;
        IConnection IContext.Connection => Connection;
        #endregion
        
        public ILogger Logger { get; }
        
        public long Id { get; }
        public string MessageType { get; }
        public JfpStream Stream { get; }
        public JfpPump Pump { get; }
        public JfpConnection Connection { get; }
        
        public void Close(CloseMode mode = CloseMode.Graceful)
        {
            Stream.Close(); // the only way we can close a context. CloseMode is redundant
        }
    }
}