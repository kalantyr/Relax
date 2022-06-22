using System.Threading;
using Kalavarda.Primitives.Process;
using Relax.DesktopClient.Interfaces;
using Relax.DesktopClient.Repository.Impl;
using Relax.DesktopClient.Services;

namespace Relax.DesktopClient
{
    internal class Context: IDesktopContext
    {
        private static readonly Context _instance = new();
        private readonly CancellationTokenSource _processorTokenSource = new();

        internal static Context Instance => _instance;

        private Context()
        {
            CharactersRepository = new CharactersRepository(AuthService);
            CharactersService = new CharactersService(AuthService, CharactersRepository);
            Processor = new MultiProcessor(60, _processorTokenSource.Token);
            CommandsService = new CommandsService(Processor, CharactersRepository);
        }

        public AuthService AuthService { get; } = new(UserSettings.Instance);

        IAuthService IDesktopContext.AuthService => AuthService;

        public CharactersService CharactersService { get; }

        ICharactersService IDesktopContext.CharactersService => CharactersService;

        public CharactersRepository CharactersRepository { get; }

        public CommandsService CommandsService { get; }

        ICommandReceiver IDesktopContext.CommandReceiver => CommandsService;

        public IProcessor Processor { get; }
    }
}
