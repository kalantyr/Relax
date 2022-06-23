using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Kalavarda.Primitives.Process;
using Relax.DesktopClient.Interfaces;
using Relax.DesktopClient.Repository.Impl;
using Relax.DesktopClient.Services;
using Relax.Server.Client;

namespace Relax.DesktopClient
{
    internal class Context: IDesktopContext
    {
        private static readonly Context _instance = new();
        private readonly CancellationTokenSource _processorTokenSource = new();

        internal static Context Instance => _instance;

        private Context()
        {
            var hostEntry = Dns.GetHostEntry(Settings.Default.UdpServer, AddressFamily.InterNetwork);
            var serverEndPoint = new IPEndPoint(hostEntry.AddressList.First(), Settings.Default.UdpServerPort);
            ServerClient = new ServerClient(serverEndPoint, Settings.Default.UdpLocalPort);

            CharactersRepository = new CharactersRepository(AuthService);
            CharactersService = new CharactersService(AuthService, CharactersRepository, ServerClient);
            Processor = new MultiProcessor(60, _processorTokenSource.Token);
            CommandsService = new CommandsService(Processor, CharactersRepository, ServerClient);

            //((ServerClient)ServerClient).ReceiveAsync(CancellationToken.None).Wait();
        }

        public AuthService AuthService { get; } = new(UserSettings.Instance);

        IAuthService IDesktopContext.AuthService => AuthService;

        public CharactersService CharactersService { get; }

        ICharactersService IDesktopContext.CharactersService => CharactersService;

        public CharactersRepository CharactersRepository { get; }

        public CommandsService CommandsService { get; }

        ICommandReceiver IDesktopContext.CommandReceiver => CommandsService;

        public IProcessor Processor { get; }

        public IServerClient ServerClient { get; }
    }
}
