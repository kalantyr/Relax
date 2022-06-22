using System;
using System.Collections.Generic;
using System.Linq;
using Kalavarda.Primitives.Process;
using Relax.Characters.Models;
using Relax.Models.Commands;

namespace Relax.DesktopClient.Processes
{
    internal class CharacterMoveProcess : IProcess, IIncompatibleProcess
    {
        private readonly CharacterInfo _charInfo;
        private readonly StartMoveCommand _startMoveCommand;

        public event Action<IProcess> Completed;

        public uint CharacterId => _charInfo.Id;

        public CharacterMoveProcess(CharacterInfo charInfo, StartMoveCommand startMoveCommand)
        {
            _charInfo = charInfo ?? throw new ArgumentNullException(nameof(charInfo));
            _startMoveCommand = startMoveCommand ?? throw new ArgumentNullException(nameof(startMoveCommand));
        }

        public void Process(TimeSpan delta)
        {
            var speed = 1;

            var (dx, dy) = CalculateOffset();

            _charInfo.Position.Offset(dx * speed * (float)delta.TotalSeconds, dy * speed * (float)delta.TotalSeconds);
        }

        private (float, float) CalculateOffset()
        {
            var dx = 0f;
            var dy = 0f;
            switch (_startMoveCommand.Direction)
            {
                case MoveDirection.Right:
                    dx = 1;
                    break;
                case MoveDirection.Left:
                    dx = -1;
                    break;
                case MoveDirection.Down:
                    dy = 1;
                    break;
                case MoveDirection.Up:
                    dy = -1;
                    break;
            }

            return (dx, dy);
        }

        public void Stop()
        {
            Completed?.Invoke(this);
        }

        public IReadOnlyCollection<IProcess> GetIncompatibleProcesses(IReadOnlyCollection<IProcess> processes)
        {
            return processes.OfType<CharacterMoveProcess>().ToArray();
        }
    }
}
