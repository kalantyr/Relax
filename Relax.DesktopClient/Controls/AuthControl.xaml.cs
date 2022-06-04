using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Relax.DesktopClient.Controls
{
    public partial class AuthControl
    {
        public AuthControl()
        {
            InitializeComponent();

            Context.Instance.AuthService.UserLoggedIn += TuneControls;
            Context.Instance.AuthService.UserLoggedOut += TuneControls;

            _cbLogin.ItemsSource = UserSettings.Instance.Logins.Select(lg => lg.Login).OrderBy(l => l);
            _cbLogin.SelectedItem = UserSettings.Instance.Logins.Select(lg => lg.Login).FirstOrDefault();

            TuneControls();
        }

        private void TuneControls()
        {
            _grdLogin.Visibility = Context.Instance.AuthService.IsUserLogged ? Visibility.Collapsed : Visibility.Visible;
            _grdLogout.Visibility = Context.Instance.AuthService.IsUserLogged ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void OnLoginClick(object sender, RoutedEventArgs e)
        {
            var tokenSource = new CancellationTokenSource(Settings.Default.AsyncTimeout);

            try
            {
                Cursor = Cursors.Wait;
                await Context.Instance.AuthService.LoginByPasswordAsync(_cbLogin.Text, _pbPassword.Password, _cbStorePwd.IsChecked == true, tokenSource.Token);
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

        private async void OnLogoutClick(object sender, RoutedEventArgs e)
        {
            var tokenSource = new CancellationTokenSource(Settings.Default.AsyncTimeout);

            try
            {
                Cursor = Cursors.Wait;
                await Context.Instance.AuthService.LogoutAsync(tokenSource.Token);
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

        private void OnLoginSelected(object sender, SelectionChangedEventArgs e)
        {
            if (!IsInitialized)
                return;

            var login = (string)_cbLogin.SelectedItem;
            var stored = UserSettings.Instance.Logins.FirstOrDefault(lg => lg.Login.Equals(login, StringComparison.InvariantCultureIgnoreCase));
            if (stored != null)
                _pbPassword.Password = stored.Password;
        }
    }
}
