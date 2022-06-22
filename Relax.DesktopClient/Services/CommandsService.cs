using System;
using System.Linq;
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

        public CommandsService(IProcessor processor, ICharactersRepository charactersRepository)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
            _charactersRepository = charactersRepository ?? throw new ArgumentNullException(nameof(charactersRepository));
        }

        public void Send(CommandBase command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            if (command is StartMoveCommand startMoveCommand)
            {
                var resolveByIdTask = _charactersRepository.ResolveByIdAsync(startMoveCommand.CharacterId, CancellationToken.None);
                resolveByIdTask.Wait();
                var character = resolveByIdTask.Result;

                var moveProcess = new CharacterMoveProcess(character.Info, startMoveCommand);
                _processor.Add(moveProcess);
            }

            if (command is StopMoveCommand stopMoveCommand)
            {
                var moveProcess = _processor.Get<CharacterMoveProcess>(cmp => cmp.CharacterId == stopMoveCommand.CharacterId).FirstOrDefault();
                moveProcess?.Stop();
            }

            CommandReceived?.Invoke(command);
        }

        public event Action<CommandBase> CommandReceived;
    }
}
