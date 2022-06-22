using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Relax.Characters.Client;
using Relax.DesktopClient.Interfaces;
using Relax.DesktopClient.Services;

namespace Relax.DesktopClient.Repository.Impl
{
    internal class CharactersRepository: ICharactersRepository
    {
        private readonly AuthService _authService;
        private readonly List<ICharacter> _characters = new();
        private readonly AuthService.HttpClientFactory _charactersHttpClientFactory = new(Settings.Default.CharactersService);

        public CharactersRepository(AuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public async Task<ICharacter> ResolveByIdAsync(uint characterId, CancellationToken cancellationToken)
        {
            // TODO: lock

            var character = _characters.FirstOrDefault(ch => ch.Info.Id == characterId);
            if (character == null)
            {
                ICharactersReadonlyClient client = new CharactersClient(_charactersHttpClientFactory);
                var result = await client.GetCharacterInfoAsync(characterId, _authService.TokenInfo.Value, cancellationToken);
                if (result.Error != null)
                    throw new Exception(result.Error.Message);
                character = new Character(result.Result);
                _characters.Add(character);
            }
            return character;
        }
    }
}
