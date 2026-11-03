using ExternalGiftCards.BusinessLayer.Common;
using ExternalGiftCards.BusinessLayer.Models;

namespace ExternalGiftCards.BusinessLayer.Interfaces
{
    public interface IPServerService
    {
        /// <summary>
        /// Reads the external provider value (p_external_provider) for the given firma/mandant, mc and key.
        /// Status: Ok (with result), BadRequest (missing input), NotFound (no row).
        /// </summary>
        Task<string> GetPServerValueAsync(string firma, string mc, string key, CancellationToken cancellationToken = default);
        Task<IServiceResponse<PServerDto>> GetPServerAsync(string firma, string mc, string key, CancellationToken cancellationToken = default);
    }
}
