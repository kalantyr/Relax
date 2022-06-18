using System.Windows.Media;
using Relax.DesktopClient.Interfaces;

namespace Relax.DesktopClient.Controls.MapObjects
{
    public partial class CharacterControl
    {
        private ICharacter _character;

        public ICharacter Character
        {
            get => _character;
            set
            {
                if (_character == value)
                    return;

                _character = value;

                if (_character != null)
                {
                    if (_character.Info.Name.StartsWith("А"))
                        _grd.Background = Brushes.DarkMagenta;
                    else
                        _grd.Background = Brushes.Chartreuse;
                }
            }
        }

        public CharacterControl()
        {
            InitializeComponent();
        }
    }
}
