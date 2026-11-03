using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExternalGiftCards.DataAccessLayer.Entity
{
    /// <summary>
    /// Read-only view of the profile_fix table (only the columns needed for lookups).
    /// Mapped as keyless: used in queries only, never tracked or saved.
    /// </summary>
    [Table("profile_fix")]
    [Keyless]
    public class ProfileFix
    {
        [Column("art")]
        public string Art { get; set; } = string.Empty;

        [Column("fk_spra_sch")]
        public string SpraSch { get; set; } = string.Empty;

        [Column("name2")]
        public string? Name2 { get; set; }
    }
}
