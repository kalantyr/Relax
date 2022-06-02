namespace Relax.Interfaces
{
    public interface IAuthController
    {
        event Action? UserLoggedIn;

        event Action? UserLoggedOut;

        bool IsUserLogged { get; }
    }
}
