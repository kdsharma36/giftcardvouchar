using System.ComponentModel.DataAnnotations.Schema;

namespace ExternalGiftCards.DataAccessLayer.Entity
{
    /// <summary>
    /// Row of the p_server table (server URL per firma/mandant and mc).
    /// </summary>
    [Table("p_server")]
    public class PServer
    {
        [Column("fk_firma_mandant")]
        public string FirmaMandant { get; set; } = string.Empty;

        [Column("mc")]
        public string Mc { get; set; } = string.Empty;

        [Column("url")]
        public string Url { get; set; } = string.Empty;

        /// <summary>p_server.[system], e.g. 'EXTPRVDR' for external provider entries.</summary>
        [Column("system")]
        public string? SystemType { get; set; }
    }
}
