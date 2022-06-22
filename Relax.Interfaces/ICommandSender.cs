using Relax.Models.Commands;

namespace Relax.DesktopClient.Interfaces
{
    public interface ICommandSender
    {
        void Send(CommandBase command);
    }
}
