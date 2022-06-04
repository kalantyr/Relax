using System.Windows;
using Relax.Characters.Models;

namespace Relax.DesktopClient.Windows
{
    public partial class CreateCharacterWindow
    {
        public CharacterInfo CharacterInfo { get; private set; }

        public CreateCharacterWindow()
        {
            InitializeComponent();
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            CharacterInfo = new CharacterInfo
            {
                Name = _tbName.Text
            };
            DialogResult = true;
        }
    }
}
