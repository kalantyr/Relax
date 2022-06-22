using System.Windows.Media;
using Kalavarda.Primitives.WPF.Controllers;
using Relax.DesktopClient.Interfaces;

namespace Relax.DesktopClient.Controls.MapObjects
{
    public partial class CharacterControl
    {
        private ICharacter _character;
        private PositionController _positionController;

        public ICharacter Character
        {
            get => _character;
            set
            {
                if (_character == value)
                    return;

                if (_character != null)
                {
                    _positionController?.Dispose();
                    _positionController = null;
                }

                _character = value;

                if (_character != null)
                {
                    if (_character.Info.Name.StartsWith("А"))
                        _grd.Background = Brushes.DarkMagenta;
                    else
                        _grd.Background = Brushes.Chartreuse;

                    _positionController = new PositionController(this, _character.Info);
                }
            }
        }

        public CharacterControl()
        {
            InitializeComponent();

            Unloaded += (sender, e) => _positionController?.Dispose();
        }
    }
}
