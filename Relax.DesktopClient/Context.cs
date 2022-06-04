using Relax.DesktopClient.Controllers;

namespace Relax.DesktopClient
{
    internal class Context
    {
        private static readonly Context _instance = new();

        public static Context Instance => _instance;

        private Context()
        {
            CharacterController = new CharacterController(AuthController);
        }

        public AuthController AuthController { get; } = new(UserSettings.Instance);

        public CharacterController CharacterController { get; }
    }
}
