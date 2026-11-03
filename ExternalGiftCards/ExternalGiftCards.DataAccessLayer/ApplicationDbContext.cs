using SnapObjects.Data;
using SnapObjects.Data.SqlServer;

namespace ExternalGiftCards.DataAccessLayer
{
    public class ApplicationDbContext : SqlServerDataContext
    {
        public ApplicationDbContext(string connectionString)
            : this(new SqlServerDataContextOptions<ApplicationDbContext>(connectionString))
        {
        }

        public ApplicationDbContext(IDataContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext(IDataContextOptions options)
            : base(options)
        {
        }
    }
}
