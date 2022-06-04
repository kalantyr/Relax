using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Relax.Characters.Models;

namespace Relax.DesktopClient.Windows
{
    public partial class GameWindow
    {
        private readonly CharacterInfo _heroInfo;
        private readonly Context _context;

        public GameWindow()
        {
            InitializeComponent();
            _context = Context.Instance;
        }

        public GameWindow(CharacterInfo heroInfo): this()
        {
            _heroInfo = heroInfo ?? throw new ArgumentNullException(nameof(heroInfo));
            Loaded += GameWindow_Loaded;
        }

        private async void GameWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var tokenSource = new CancellationTokenSource(Settings.Default.AsyncTimeout);

            try
            {
                Cursor = Cursors.Wait;
                await _context.CharactersService.EnterAsync(_heroInfo, tokenSource.Token);
                _locationControl.Visibility = Visibility.Visible;
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

        protected override void OnClosing(CancelEventArgs e)
        {
            _locationControl.Visibility = Visibility.Collapsed;

            var tokenSource = new CancellationTokenSource(Settings.Default.AsyncTimeout);
            _context.CharactersService.ExitAsync(tokenSource.Token).Wait(tokenSource.Token);

            base.OnClosing(e);
        }
    }
}
