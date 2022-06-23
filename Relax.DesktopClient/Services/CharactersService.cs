using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Relax.Characters.Client;
using Relax.Characters.Models;
using Relax.DesktopClient.Interfaces;
using Relax.DesktopClient.Repository;
using Relax.Server.Client;

namespace Relax.DesktopClient.Services
{
    internal class CharactersService: ICharactersService
    {
        private readonly AuthService _authService;
        private readonly ICharactersRepository _charactersRepository;
        private readonly AuthService.HttpClientFactory _charactersHttpClientFactory = new(Settings.Default.CharactersService);
        private Character _hero;
        private Timer _timer;
        private readonly ICollection<uint> _onlineCharacters = new List<uint>();
        private readonly IServerClient _serverClient;

        public CharactersService(AuthService authService, ICharactersRepository charactersRepository, IServerClient serverClient)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _charactersRepository = charactersRepository ?? throw new ArgumentNullException(nameof(charactersRepository));
            _serverClient = serverClient ?? throw new ArgumentNullException(nameof(serverClient));
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
            var character = await _charactersRepository.ResolveByIdAsync(charId, cancellationToken);
            return character.Info;
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
            var getIdsOnline = await _serverClient.GetOnlineCharacterIdsAsync(_authService.TokenInfo.Value, cancellationToken);
            if (getIdsOnline.Error != null)
                throw new Exception(getIdsOnline.Error.Message);

            cancellationToken.ThrowIfCancellationRequested();

            var result = await _serverClient.ConnectAsync(info.Id, _authService.TokenInfo.Value, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);

            _timer = new Timer(OnTimer, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            Hero = new Character(info);
        }

        private void OnTimer(object objectState)
        {
            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            var task = _serverClient.GetOnlineCharacterIdsAsync(_authService.TokenInfo.Value, tokenSource.Token);

            task.Wait(tokenSource.Token);
            var getIdsOnline = task.Result;

            if (getIdsOnline.Error != null)
                throw new Exception(getIdsOnline.Error.Message);

            foreach (var id in getIdsOnline.Result)
                if (id != Hero.Info.Id)
                    if (!_onlineCharacters.Contains(id))
                    {
                        _onlineCharacters.Add(id);
                        if (CharacterOnline != null)
                        {
                            var tokenSource2 = new CancellationTokenSource(TimeSpan.FromSeconds(1));
                            var resolveTask = _charactersRepository.ResolveByIdAsync(id, tokenSource2.Token);
                            resolveTask.Wait(tokenSource2.Token);
                            CharacterOnline.Invoke(resolveTask.Result);
                        }
                    }
        }

        public async Task ExitAsync(CancellationToken cancellationToken)
        {
            await _timer.DisposeAsync();
            _timer = null;
            _onlineCharacters.Clear();

            var result = await _serverClient.DisconnectAsync(_authService.TokenInfo.Value, cancellationToken);
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

        public event Action<ICharacter> CharacterOnline;

        public event Action<ICharacter> CharacterOffline;
    }
}
