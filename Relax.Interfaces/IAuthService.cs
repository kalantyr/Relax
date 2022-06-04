namespace Relax.DesktopClient.Interfaces
{
    public interface IAuthService
    {
        event Action? UserLoggedIn;

        event Action? UserLoggedOut;

        bool IsUserLogged { get; }
    }
}
