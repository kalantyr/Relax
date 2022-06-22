using System.Threading;
using System.Threading.Tasks;
using Relax.DesktopClient.Interfaces;

namespace Relax.DesktopClient.Repository
{
    internal interface ICharactersRepositiryReadonly
    {
        Task<ICharacter> ResolveByIdAsync(uint characterId, CancellationToken cancellationToken);
    }

    internal interface ICharactersRepository: ICharactersRepositiryReadonly
    {
    }
}
