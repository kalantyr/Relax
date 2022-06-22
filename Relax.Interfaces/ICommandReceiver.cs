using Relax.Models.Commands;

namespace Relax.DesktopClient.Interfaces
{
    public interface ICommandReceiver
    {
        event Action<CommandBase> CommandReceived;
    }
}
