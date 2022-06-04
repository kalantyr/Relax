namespace Relax.DesktopClient.Interfaces
{
    public interface IDesktopContext
    {
        IAuthController AuthController { get; }

        ICharacterController CharacterController { get; }
    }
}
