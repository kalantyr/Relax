using Relax.DesktopClient.Controllers;
using Relax.DesktopClient.Interfaces;

namespace Relax.DesktopClient
{
    internal class Context: IDesktopContext
    {
        private static readonly Context _instance = new();

        internal static Context Instance => _instance;

        private Context()
        {
            CharacterController = new CharacterController(AuthController);
        }

        public AuthController AuthController { get; } = new(UserSettings.Instance);

        IAuthController IDesktopContext.AuthController => AuthController;

        public CharacterController CharacterController { get; }

        ICharacterController IDesktopContext.CharacterController => CharacterController;
    }
}
