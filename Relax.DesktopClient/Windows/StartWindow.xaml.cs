using System;
using System.Windows;

namespace Relax.DesktopClient.Windows
{
    public partial class StartWindow
    {
        public StartWindow()
        {
            InitializeComponent();

            Context.Instance.AuthService.UserLoggedIn += TuneControls;
            Context.Instance.AuthService.UserLoggedOut += TuneControls;

            _characterSelector.Selected += characterInfo =>
            {
                new GameWindow(characterInfo) { Owner = this }.ShowDialog();
            };

            TuneControls();
        }

        private void TuneControls()
        {
            _charactersGroup.Visibility =
                Context.Instance.AuthService.IsUserLogged ? Visibility.Visible : Visibility.Collapsed;
        }

        protected override void OnClosed(EventArgs e)
        {
            Context.Instance.AuthService.UserLoggedIn -= TuneControls;
            Context.Instance.AuthService.UserLoggedOut -= TuneControls;

            base.OnClosed(e);
        }
    }
}
