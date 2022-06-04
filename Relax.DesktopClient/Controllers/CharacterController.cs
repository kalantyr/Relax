using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Relax.Characters.Client;
using Relax.Characters.Models;
using Relax.DesktopClient.Interfaces;
using Relax.Server.Client;

namespace Relax.DesktopClient.Controllers
{
    internal class CharacterController: ICharacterController
    {
        private readonly AuthController _authController;
        private readonly AuthController.HttpClientFactory _httpClientFactory = new(Settings.Default.CharactersService);
        private Character _hero;

        public CharacterController(AuthController authController)
        {
            _authController = authController ?? throw new ArgumentNullException(nameof(authController));
        }

        public async Task<IReadOnlyCollection<uint>> GetMyCharactersIdsAsync(CancellationToken cancellationToken)
        {
            ICharactersReadonlyClient client = new CharactersClient(_httpClientFactory);
            var result = await client.GetMyCharactersIdsAsync(_authController.TokenInfo.Value, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);
            return result.Result;
        }

        public async Task<CharacterInfo> GetCharacterInfoAsync(uint charId, CancellationToken cancellationToken)
        {
            ICharactersReadonlyClient client = new CharactersClient(_httpClientFactory);
            var result = await client.GetCharacterInfoAsync(charId, _authController.TokenInfo.Value, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);
            return result.Result;
        }

        public async Task<uint> CreateAsync(CharacterInfo info, CancellationToken cancellationToken)
        {
            ICharactersClient client = new CharactersClient(_httpClientFactory);
            var result = await client.CreateAsync(info, _authController.TokenInfo.Value, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);
            return result.Result;
        }

        public async Task EnterAsync(CharacterInfo info, CancellationToken cancellationToken)
        {
            IServerClient client = new ServerClient();
            var result = await client.ConnectAsync(info.Id, _authController.TokenInfo.Value, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);
            Hero = new Character(info);
        }

        public async Task ExitAsync(CancellationToken cancellationToken)
        {
            IServerClient client = new ServerClient();
            var result = await client.DisconnectAsync(_authController.TokenInfo.Value, cancellationToken);
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

        ICharacter ICharacterController.Hero => Hero;

        public event Action<ICharacter, ICharacter> HeroChanged;

        public event Action<uint> CharacterOnline;

        public event Action<uint> CharacterOffline;
    }
}
