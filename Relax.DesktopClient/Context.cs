using Relax.DesktopClient.Interfaces;
using Relax.DesktopClient.Services;

namespace Relax.DesktopClient
{
    internal class Context: IDesktopContext
    {
        private static readonly Context _instance = new();

        internal static Context Instance => _instance;

        private Context()
        {
            CharactersService = new CharactersService(AuthService);
        }

        public AuthService AuthService { get; } = new(UserSettings.Instance);

        IAuthService IDesktopContext.AuthService => AuthService;

        public CharactersService CharactersService { get; }

        ICharactersService IDesktopContext.CharactersService => CharactersService;
    }
}
