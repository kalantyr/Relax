using System;
using System.ComponentModel;
using System.Threading;
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

        private async void GameWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var tokenSource = new CancellationTokenSource(Settings.Default.AsyncTimeout);

            try
            {
                Cursor = Cursors.Wait;
                await Context.Instance.CharacterController.EnterAsync(_heroInfo, tokenSource.Token);
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
            var cancellationTokenSource = new CancellationTokenSource(Settings.Default.AsyncTimeout);
            _context.CharacterController.ExitAsync(cancellationTokenSource.Token).Wait(cancellationTokenSource.Token);

            base.OnClosing(e);
        }
    }
}
