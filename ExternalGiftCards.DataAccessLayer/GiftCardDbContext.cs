using ExternalGiftCards.DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;

namespace ExternalGiftCards.DataAccessLayer
{
    /// <summary>
    /// EF Core context for the tenant database.
    /// </summary>
    public class GiftCardDbContext : DbContext
    {
        public GiftCardDbContext(DbContextOptions<GiftCardDbContext> options)
            : base(options)
        {
        }

        public DbSet<PServer> PServers => Set<PServer>();
        public DbSet<PExternalProvider> PExternalProviders => Set<PExternalProvider>();
        public DbSet<ProfileFix> ProfileFixes => Set<ProfileFix>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Table/column names come from the [Table]/[Column] attributes on PServer.
            modelBuilder.Entity<PServer>(entity =>
            {
                entity.HasKey(e => new { e.FirmaMandant, e.Mc });
            });

            modelBuilder.Entity<PExternalProvider>(entity =>
            {
                entity.HasKey(e => new { e.FirmaMandant, e.PServerMc, e.Key });
            });

            // ProfileFix is [Keyless] - query only.
        }
    }
}
