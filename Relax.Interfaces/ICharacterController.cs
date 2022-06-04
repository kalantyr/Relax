﻿namespace Relax.DesktopClient.Interfaces
{
    public interface ICharacterController
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
        event Action<uint> CharacterOnline;

        /// <summary>
        /// Персонаж у указанным ID вышел из онлайна
        /// </summary>
        event Action<uint> CharacterOffline;
    }
}
