using Relax.Characters.Models;

namespace Relax.DesktopClient.Interfaces
{
    public interface ICharactersService
    {
        /// <summary>
        /// Текущий персонаж
        /// </summary>
        ICharacter Hero { get; }

        /// <summary>
        /// <see cref="Hero"/> сменился
        /// </summary>
        event Action<ICharacter, ICharacter> HeroChanged;

        /// <summary>
        /// Персонаж у указанным ID зашёл в онлайн
        /// </summary>
        event Action<ICharacter> CharacterOnline;

        /// <summary>
        /// Персонаж у указанным ID вышел из онлайна
        /// </summary>
        event Action<ICharacter> CharacterOffline;
    }
}
