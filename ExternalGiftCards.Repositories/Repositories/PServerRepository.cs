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

        public Task<PExternalProvider?> GetPServerAsync(string firma, string mc, string key, CancellationToken cancellationToken = default)
        {
            var query =
                from ep in _context.PExternalProviders.AsNoTracking()
                join ps in _context.PServers.AsNoTracking()
                    on new { FirmaMandant = ep.FirmaMandant, Mc = ep.PServerMc }
                    equals new { FirmaMandant = ps.FirmaMandant, Mc = ps.Mc }
                join pf in _context.ProfileFixes.AsNoTracking()
                    on ep.Key equals pf.Name2
                where pf.Art == ExternalProviderKeyArt
                   && pf.SpraSch == ProfileLanguage
                   && ps.SystemType == ExternalProviderSystem
                   && ps.FirmaMandant == firma
                   && ps.Mc == mc
                   && ep.Key == key
                select ep;

            return query.FirstOrDefaultAsync(cancellationToken);
        }
    }
}
