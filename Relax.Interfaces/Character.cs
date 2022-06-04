using Relax.Characters.Models;

namespace Relax.DesktopClient.Interfaces
{
    public class Character: ICharacter
    {
        public Character(CharacterInfo info)
        {
            Info = info ?? throw new ArgumentNullException(nameof(info));
        }

        public CharacterInfo Info { get; }
    }
}
