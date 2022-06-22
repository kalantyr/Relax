using System;
using System.Windows;
using System.Windows.Input;
using Relax.DesktopClient.Interfaces;
using Relax.Models.Commands;

namespace Relax.DesktopClient.Controllers
{
    internal class InputController: IDisposable
    {
        private readonly UIElement _uiElement;
        private readonly ICharactersService _charactersService;
        private readonly ICommandSender _commandSender;

        public InputController(UIElement uiElement, ICharactersService charactersService, ICommandSender commandSender)
        {
            _uiElement = uiElement ?? throw new ArgumentNullException(nameof(uiElement));
            _charactersService = charactersService ?? throw new ArgumentNullException(nameof(charactersService));
            _commandSender = commandSender ?? throw new ArgumentNullException(nameof(commandSender));

            _uiElement.KeyDown += OnKeyDown;
            _uiElement.KeyUp += OnKeyUp;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled)
                return;

            switch (e.Key)
            {
                case Key.D:
                    if (_charactersService.Hero?.Info != null)
                    {
                        var cmd = new StartMoveCommand(_charactersService.Hero.Info.Id, _charactersService.Hero.Info.Position, MoveDirection.Right);
                        _commandSender.Send(cmd);
                        e.Handled = true;
                    }
                    break;
                case Key.A:
                    if (_charactersService.Hero?.Info != null)
                    {
                        var cmd = new StartMoveCommand(_charactersService.Hero.Info.Id, _charactersService.Hero.Info.Position, MoveDirection.Left);
                        _commandSender.Send(cmd);
                        e.Handled = true;
                    }
                    break;
                case Key.S:
                    if (_charactersService.Hero?.Info != null)
                    {
                        var cmd = new StartMoveCommand(_charactersService.Hero.Info.Id, _charactersService.Hero.Info.Position, MoveDirection.Down);
                        _commandSender.Send(cmd);
                        e.Handled = true;
                    }
                    break;
                case Key.W:
                    if (_charactersService.Hero?.Info != null)
                    {
                        var cmd = new StartMoveCommand(_charactersService.Hero.Info.Id, _charactersService.Hero.Info.Position, MoveDirection.Up);
                        _commandSender.Send(cmd);
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Handled)
                return;

            switch (e.Key)
            {
                case Key.W:
                case Key.A:
                case Key.S:
                case Key.D:
                    if (_charactersService.Hero?.Info != null)
                    {
                        var cmd = new StopMoveCommand(_charactersService.Hero.Info.Id, _charactersService.Hero.Info.Position);
                        _commandSender.Send(cmd);
                        e.Handled = true;
                    }
                    break;
            }
        }

        public void Dispose()
        {
            _uiElement.KeyDown -= OnKeyDown;
            _uiElement.KeyUp -= OnKeyUp;
        }
    }
}
