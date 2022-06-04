using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Relax.Characters.Models;
using Relax.DesktopClient.Controllers;
using Relax.DesktopClient.Windows;

namespace Relax.DesktopClient.Controls
{
    public partial class CharacterSelectorControl
    {
        private readonly ICollection<CharacterInfo> _characters = new ObservableCollection<CharacterInfo>();
        private readonly CharacterController _characterController;

        public CharacterSelectorControl()
        {
            InitializeComponent();

            _characterController = Context.Instance.CharacterController;
            _lb.ItemsSource = _characters;

            Context.Instance.AuthController.UserLoggedIn += AuthController_UserLoggedIn;
            Unloaded += CharacterSelectorControl_Unloaded;
        }

        private void CharacterSelectorControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Context.Instance.AuthController.UserLoggedIn -= AuthController_UserLoggedIn;
        }

        private async void AuthController_UserLoggedIn()
        {
            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            try
            {
                Cursor = Cursors.Wait;
                _characters.Clear();
                var charIds = await _characterController.GetMyCharactersIdsAsync(tokenSource.Token);
                foreach (var charId in charIds) // TODO: parallel
                {
                    var charInfo = await _characterController.GetCharacterInfoAsync(charId, tokenSource.Token);
                    _characters.Add(charInfo);
                }
                _lb.ItemsSource = _characters;
            }
            catch (Exception exception)
            {
                App.ShowError(exception);
            }
            finally
            {
                Cursor = null;
            }
        }

        private async void OnCreateClick(object sender, RoutedEventArgs e)
        {
            var window = new CreateCharacterWindow { Owner = Window.GetWindow(this) };
            if (window.ShowDialog() == true)
            {
                var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                try
                {
                    Cursor = Cursors.Wait;
                    var charId = await _characterController.CreateAsync(window.CharacterInfo, tokenSource.Token);
                    var charInfo = await _characterController.GetCharacterInfoAsync(charId, tokenSource.Token);
                    _characters.Add(charInfo);
                }
                catch (Exception exception)
                {
                    App.ShowError(exception);
                }
                finally
                {
                    Cursor = null;
                }
            }
        }
    }
}
