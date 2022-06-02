using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;
using Relax.Characters.Models;

namespace Relax.Controls
{
    public partial class CharacterSelectorControl
    {
        public CharacterSelectorControl()
        {
            InitializeComponent();

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
                _lb.ItemsSource = null;
                var charIds = await Context.Instance.CharacterController.GetAllCharactersIdsAsync(tokenSource.Token);
                var characters = new List<CharacterInfo>();
                foreach (var charId in charIds) // TODO: parallel
                {
                    var charInfo = await Context.Instance.CharacterController.GetCharacterInfoAsync(charId, tokenSource.Token);
                    characters.Add(charInfo);
                }
                _lb.ItemsSource = characters;
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
