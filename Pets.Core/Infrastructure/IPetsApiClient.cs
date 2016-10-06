using Pets.Core.Models;
using System.Threading.Tasks;

namespace Pets.Core.Infrastructure
{
    public interface IPetsApiClient
    {
        Task<PetsApiResponse> GetAllPetsOwners (string requestUri);
    }
}
