using System;
using System.Windows;

namespace Relax.DesktopClient.Windows
{
    public partial class StartWindow
    {
        public StartWindow()
        {
            InitializeComponent();

            Context.Instance.AuthController.UserLoggedIn += TuneControls;
            Context.Instance.AuthController.UserLoggedOut += TuneControls;

            TuneControls();
        }

        private void TuneControls()
        {
            _charactersGroup.Visibility =
                Context.Instance.AuthController.IsUserLogged ? Visibility.Visible : Visibility.Collapsed;
        }

        protected override void OnClosed(EventArgs e)
        {
            Context.Instance.AuthController.UserLoggedIn -= TuneControls;
            Context.Instance.AuthController.UserLoggedOut -= TuneControls;

            base.OnClosed(e);
        }
    }
}
