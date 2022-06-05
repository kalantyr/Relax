using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Relax.Characters.Client;
using Relax.Characters.Models;
using Relax.DesktopClient.Interfaces;
using Relax.Server.Client;

namespace Relax.DesktopClient.Services
{
    internal class CharactersService: ICharactersService
    {
        private readonly AuthService _authService;
        private readonly AuthService.HttpClientFactory _charactersHttpClientFactory = new(Settings.Default.CharactersService);
        private readonly AuthService.HttpClientFactory _serverHttpClientFactory = new(Settings.Default.HttpServer);
        private Character _hero;

        public CharactersService(AuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public async Task<IReadOnlyCollection<uint>> GetMyCharactersIdsAsync(CancellationToken cancellationToken)
        {
            ICharactersReadonlyClient client = new CharactersClient(_charactersHttpClientFactory);
            var result = await client.GetMyCharactersIdsAsync(_authService.TokenInfo.Value, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);
            return result.Result;
        }

        public async Task<CharacterInfo> GetCharacterInfoAsync(uint charId, CancellationToken cancellationToken)
        {
            ICharactersReadonlyClient client = new CharactersClient(_charactersHttpClientFactory);
            var result = await client.GetCharacterInfoAsync(charId, _authService.TokenInfo.Value, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);
            return result.Result;
        }

        public async Task<uint> CreateAsync(CharacterInfo info, CancellationToken cancellationToken)
        {
            ICharactersClient client = new CharactersClient(_charactersHttpClientFactory);
            var result = await client.CreateAsync(info, _authService.TokenInfo.Value, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);
            return result.Result;
        }

        public async Task EnterAsync(CharacterInfo info, CancellationToken cancellationToken)
        {
            IServerClient client = new ServerClient(_serverHttpClientFactory);
            var result = await client.ConnectAsync(info.Id, _authService.TokenInfo.Value, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);
            Hero = new Character(info);
        }

        public async Task ExitAsync(CancellationToken cancellationToken)
        {
            IServerClient client = new ServerClient(_serverHttpClientFactory);
            var result = await client.DisconnectAsync(_authService.TokenInfo.Value, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);
            Hero = null;
        }

        public Character Hero
        {
            get => _hero;
            private set
            {
                if (_hero == value)
                    return;

                var oldValue = _hero;
                _hero = value;
                HeroChanged?.Invoke(oldValue, _hero);
            }
        }

        ICharacter ICharactersService.Hero => Hero;

        public event Action<ICharacter, ICharacter> HeroChanged;

        public event Action<uint> CharacterOnline;

        public event Action<uint> CharacterOffline;
    }
}
