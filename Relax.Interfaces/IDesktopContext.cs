namespace Relax.DesktopClient.Interfaces
{
    public interface IDesktopContext
    {
        IAuthService AuthService { get; }

        ICharactersService CharactersService { get; }
    }
}
