using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ultz.Jfp.IO;
using Ultz.SimpleServer.Internals;

namespace Ultz.Jfp.SimpleServer
{
    public class JfpProtocol : IProtocol
    {
        public Task HandleConnectionAsync(IConnection connection, ILogger logger)
        {
            var pump = new JfpPump(connection.Stream);
            pump.OnCommand += (sender, args) =>
            {
                ContextCreated?.Invoke(sender,
                    new ContextEventArgs(new JfpContext(new JfpConnection(connection,pump), logger, (JfpStream) args.DataStream)));
            };
            pump.Start();
            return Task.CompletedTask;
        }

        public IListener CreateDefaultListener(IPEndPoint endpoint)
        {
            return new TcpConnectionListener(endpoint);
        }

        IAttributeHandlerResolver IProtocol.AttributeHandlerResolver => AttributeHandlerResolver;
        public JfpAttributeHandlerResolver AttributeHandlerResolver { get; } = new JfpAttributeHandlerResolver();
        public IMethodResolver MethodResolver => new JustAMethodResolver();
        public event EventHandler<ContextEventArgs> ContextCreated;
    }
}