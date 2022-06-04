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
        private readonly CharactersService _charactersService;

        public CharacterInfo SelectedCharacter => _lb.SelectedItem as CharacterInfo;

        public event Action<CharacterInfo> Selected;

        public CharacterSelectorControl()
        {
            InitializeComponent();

            _charactersService = Context.Instance.CharactersService;
            _lb.ItemsSource = _characters;

            Context.Instance.AuthService.UserLoggedIn += AuthController_UserLoggedIn;
            Unloaded += CharacterSelectorControl_Unloaded;
        }

        private void CharacterSelectorControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Context.Instance.AuthService.UserLoggedIn -= AuthController_UserLoggedIn;
        }

        private async void AuthController_UserLoggedIn()
        {
            var tokenSource = new CancellationTokenSource(Settings.Default.AsyncTimeout);

            try
            {
                Cursor = Cursors.Wait;
                _characters.Clear();
                var charIds = await _charactersService.GetMyCharactersIdsAsync(tokenSource.Token);
                foreach (var charId in charIds) // TODO: parallel
                {
                    var charInfo = await _charactersService.GetCharacterInfoAsync(charId, tokenSource.Token);
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
                var tokenSource = new CancellationTokenSource(Settings.Default.AsyncTimeout);

                try
                {
                    Cursor = Cursors.Wait;
                    var charId = await _charactersService.CreateAsync(window.CharacterInfo, tokenSource.Token);
                    var charInfo = await _charactersService.GetCharacterInfoAsync(charId, tokenSource.Token);
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

        private void OnSelectClick(object sender, RoutedEventArgs e)
        {
            if (SelectedCharacter != null)
                Selected?.Invoke(SelectedCharacter);
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCharacter != null)
                OnSelectClick(sender, e);
        }
    }
}
