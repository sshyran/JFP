using Ultz.Jfp;
using Ultz.Jfp.SimpleServer;
using Ultz.SimpleServer.Common;
using Ultz.SimpleServer.Internals;

namespace TestApp2
{
    public class TestService : Service
    {
        protected override void BeforeStart()
        {
            ((JfpProtocol)Protocol).AttributeHandlerResolver.Register<HelloWorldTestAttribute>();
            RegisterHandlers(new TestHandlers());
        }

        protected override void OnStart()
        {
        }

        protected override void AfterStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override void BeforeStop()
        {
        }

        protected override void AfterStop()
        {
        }

        protected override void OnError(ErrorType type, IContext context)
        {
            ((JfpProtocol)Protocol).AttributeHandlerResolver.Deregister<HelloWorldTestAttribute>();
        }

        public override IProtocol Protocol => new JfpProtocol();
    }
}