using System.ComponentModel.DataAnnotations.Schema;

namespace ExternalGiftCards.DataAccessLayer.Entity
{
    /// <summary>
    /// Row of the p_external_provider table (key/value settings per p_server entry).
    /// Linked to p_server via (fk_firma_mandant, fk_pserver_mc) -> (fk_firma_mandant, mc).
    /// </summary>
    [Table("p_external_provider")]
    public class PExternalProvider
    {
        [Column("fk_firma_mandant")]
        public string FirmaMandant { get; set; } = string.Empty;

        [Column("fk_pserver_mc")]
        public string PServerMc { get; set; } = string.Empty;

        [Column("key")]
        public string Key { get; set; } = string.Empty;

        [Column("value")]
        public string? Value { get; set; }

        [Column("aend_datum")]
        public DateTime? AendDatum { get; set; }

        [Column("aend_login")]
        public string? AendLogin { get; set; }

        [Column("aend_mandant")]
        public string? AendMandant { get; set; }
    }
}
