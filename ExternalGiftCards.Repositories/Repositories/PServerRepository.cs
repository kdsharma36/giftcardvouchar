using ExternalGiftCards.DataAccessLayer;
using ExternalGiftCards.Repositories.Interfaces;
using ExternalGiftCards.DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;

namespace ExternalGiftCards.Repositories.Repositories
{
    public class PServerRepository : IPServerRepository
    {
        private const string ExternalProviderSystem = "EXTPRVDR";
        private const string ExternalProviderKeyArt = "EXPKY";
        private const string ProfileLanguage = "GER";

        private readonly GiftCardDbContext _context;

        public PServerRepository(GiftCardDbContext context)
        {
            _context = context;
        }

        public Task<PExternalProvider?> GetPServerAsync(
    string firma, string mc, string key, CancellationToken cancellationToken = default)
        {            
            return _context.PExternalProviders
                .AsNoTracking()
                .Where(ep => ep.FirmaMandant == firma
                          && ep.PServerMc == mc
                          && ep.Key == key)
                .Where(ep => _context.PServers.Any(ps =>
                          ps.FirmaMandant == ep.FirmaMandant
                       && ps.Mc == ep.PServerMc
                       && ps.SystemType == ExternalProviderSystem))
                .Where(ep => _context.ProfileFixes.Any(pf =>
                          pf.Name2 == ep.Key
                       && pf.Art == ExternalProviderKeyArt
                       && pf.SpraSch == ProfileLanguage))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
