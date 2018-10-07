using Ultz.SimpleServer.Internals;

namespace Ultz.Jfp.SimpleServer
{
    public class JustAMethodResolver : IMethodResolver
    {
        public IMethod GetMethod(byte[] id)
        {
            return new JustAMethod(){Id=id};
        }

        public class JustAMethod : IMethod
        {
            public byte[] Id { get; set; }
        }
    }
}