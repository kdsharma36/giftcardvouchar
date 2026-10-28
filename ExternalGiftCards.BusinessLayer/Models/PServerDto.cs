namespace ExternalGiftCards.BusinessLayer.Models
{
    public class PServerDto
    {
        public string FirmaMandant { get; set; } = string.Empty;
        public string PServerMc { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string? Value { get; set; }
        public DateTime? AendDatum { get; set; }
        public string? AendLogin { get; set; }
        public string? AendMandant { get; set; }
        public string? Name2 { get; set; }
    }
}
