using Ultz.Jfp.IO;

namespace Ultz.Jfp
{
    public interface ICommandHandler
    {
        string CommandName { get; }
        void Handle(JfpMessage msg, JfpStream stream);
    }
}