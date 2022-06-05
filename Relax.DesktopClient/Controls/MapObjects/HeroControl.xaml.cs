using Relax.DesktopClient.Interfaces;

namespace Relax.DesktopClient.Controls.MapObjects
{
    public partial class HeroControl
    {
        public ICharacter Hero
        {
            get;
            set;
        }

        public HeroControl()
        {
            InitializeComponent();
        }
    }
}
