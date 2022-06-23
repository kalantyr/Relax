using System;
using System.Linq;
using System.Threading;
using Kalavarda.Primitives.Process;
using Relax.DesktopClient.Interfaces;
using Relax.DesktopClient.Processes;
using Relax.DesktopClient.Repository;
using Relax.Models.Commands;
using Relax.Server.Client;

namespace Relax.DesktopClient.Services
{
    internal class CommandsService: ICommandSender, ICommandReceiver
    {
        private readonly IProcessor _processor;
        private readonly ICharactersRepository _charactersRepository;
        private readonly IServerClient _serverClient;

        public CommandsService(IProcessor processor, ICharactersRepository charactersRepository, IServerClient serverClient)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
            _charactersRepository = charactersRepository ?? throw new ArgumentNullException(nameof(charactersRepository));
            _serverClient = serverClient;
        }

        public void Send(CommandBase command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            Process(command);
            
            _serverClient.SendAsync(command, CancellationToken.None).Wait();

            CommandReceived?.Invoke(command);
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
