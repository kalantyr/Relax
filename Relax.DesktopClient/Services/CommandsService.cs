using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Kalavarda.Primitives.Process;
using Relax.DesktopClient.Interfaces;
using Relax.DesktopClient.Processes;
using Relax.DesktopClient.Repository;
using Relax.Models.Commands;

namespace Relax.DesktopClient.Services
{
    internal class CommandsService: ICommandSender, ICommandReceiver
    {
        private readonly IProcessor _processor;
        private readonly ICharactersRepository _charactersRepository;
        private readonly IPEndPoint _serverEndPoint;

        public CommandsService(IProcessor processor, ICharactersRepository charactersRepository)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
            _charactersRepository = charactersRepository ?? throw new ArgumentNullException(nameof(charactersRepository));

            var hostEntry = Dns.GetHostEntry(Settings.Default.UdpServer, AddressFamily.InterNetwork);
            _serverEndPoint = new(hostEntry.AddressList.First(), Settings.Default.UdpPort);
        }

        public void Send(CommandBase command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            Process(command);
            
            SendToServer(command);

            CommandReceived?.Invoke(command);
        }

        private void SendToServer(CommandBase command)
        {
            using var udpClient = new UdpClient();
            try
            {
                udpClient.Connect(_serverEndPoint); // TODO: нужно ли?
                udpClient.Send(command.Serialize());
            }
            finally
            {
                udpClient.Close();
            }
        }

        private void Process(CommandBase command)
        {
            if (command is StartMoveCommand startMoveCommand)
            {
                var resolveByIdTask =
                    _charactersRepository.ResolveByIdAsync(startMoveCommand.CharacterId, CancellationToken.None);
                resolveByIdTask.Wait();
                var character = resolveByIdTask.Result;

                var moveProcess = new CharacterMoveProcess(character.Info, startMoveCommand);
                _processor.Add(moveProcess);
            }

            if (command is StopMoveCommand stopMoveCommand)
            {
                var moveProcess = _processor.Get<CharacterMoveProcess>(cmp => cmp.CharacterId == stopMoveCommand.CharacterId)
                    .FirstOrDefault();
                moveProcess?.Stop();
            }
        }

        public event Action<CommandBase> CommandReceived;
    }
}
