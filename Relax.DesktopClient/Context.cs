using Relax.DesktopClient.Interfaces;
using Relax.DesktopClient.Repository.Impl;
using Relax.DesktopClient.Services;

namespace Relax.DesktopClient
{
    internal class Context: IDesktopContext
    {
        private static readonly Context _instance = new();

        internal static Context Instance => _instance;

        private Context()
        {
            CharactersRepositiry = new CharactersRepositiry(AuthService);
            CharactersService = new CharactersService(AuthService, CharactersRepositiry);
        }

        public AuthService AuthService { get; } = new(UserSettings.Instance);

        IAuthService IDesktopContext.AuthService => AuthService;

        public CharactersService CharactersService { get; }

        ICharactersService IDesktopContext.CharactersService => CharactersService;

        public CharactersRepositiry CharactersRepositiry { get; }
    }
}
