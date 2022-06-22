using System.Windows.Media;
using Kalavarda.Primitives.WPF.Controllers;
using Relax.DesktopClient.Interfaces;

namespace Relax.DesktopClient.Controls.MapObjects
{
    public partial class HeroControl
    {
        private ICharacter _hero;
        private PositionController _positionController;

        public ICharacter Hero
        {
            get => _hero;
            set
            {
                if (_hero == value)
                    return;

                if (_hero != null)
                {
                    _positionController?.Dispose();
                    _positionController = null;
                }

                _hero = value;

                if (_hero != null)
                {
                    _grd.Background = _hero.Info.Name.StartsWith("А")
                        ? Brushes.DarkMagenta
                        : Brushes.Chartreuse;

                    _positionController = new PositionController(this, _hero.Info);
                }
            }
        }

        public HeroControl()
        {
            InitializeComponent();

            Unloaded += (sender, e) => _positionController?.Dispose();
        }
    }
}
