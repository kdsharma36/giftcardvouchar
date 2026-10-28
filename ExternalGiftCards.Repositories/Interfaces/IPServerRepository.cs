using ExternalGiftCards.DataAccessLayer.Entity;

namespace ExternalGiftCards.Repositories.Interfaces
{
    public interface IPServerRepository
    {
        /// <summary>
        /// Returns the p_external_provider value for the given firma/mandant, mc and key
        /// (p_server.system = 'EXTPRVDR', key must exist in profile_fix art 'EXPKY' / 'GER'),
        /// or null if none exists.
        /// </summary>
        Task<PExternalProvider?> GetPServerAsync(string firma, string mc, string key, CancellationToken cancellationToken = default);
    }
}
