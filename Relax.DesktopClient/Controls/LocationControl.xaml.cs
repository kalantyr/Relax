using Relax.DesktopClient.Controls.MapObjects;
using Relax.DesktopClient.Interfaces;

namespace Relax.Controls
{
    public partial class LocationControl
    {
        private ICharactersService _charactersService;
        private HeroControl _heroControl;

        public ICharactersService CharactersService
        {
            get => _charactersService;
            set
            {
                if (_charactersService == value)
                    return;

                if (_charactersService != null)
                    _charactersService.HeroChanged -= OnHeroChanged;

                _charactersService = value;

                if (_charactersService != null)
                {
                    _charactersService.HeroChanged += OnHeroChanged;
                    OnHeroChanged(null, _charactersService.Hero);
                }
            }
        }

        private void OnHeroChanged(ICharacter oldChar, ICharacter newChar)
        {
            if (_heroControl != null)
            {
                _canvas.Children.Remove(_heroControl);
                _heroControl = null;
            }

            if (newChar != null)
            {
                _heroControl = new HeroControl { Hero = newChar };
                _canvas.Children.Add(_heroControl);
            }
        }

        public LocationControl()
        {
            InitializeComponent();
        }
    }
}
