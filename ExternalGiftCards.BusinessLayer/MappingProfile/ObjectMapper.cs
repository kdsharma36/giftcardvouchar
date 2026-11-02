using ExternalGiftCards.BusinessLayer.Models;
using ExternalGiftCards.BusinessLayer.Models.External;
using ExternalGiftCards.DataAccessLayer.Entity;

namespace ExternalGiftCards.BusinessLayer.MappingProfile
{
    /// <summary>
    /// Replaces AutoMapper. Add explicit, compile-safe extension methods here for every mapping,
    /// e.g. <c>public static XDto ToXDto(this DS_X src)</c>.
    ///
    /// USAGE - collection:
    ///     List&lt;XDto&gt; list = dsItems.ToMappedList(x => x.ToXDto());
    /// </summary>
    public static class ObjectMapper
    {
        public static List<TDest> ToMappedList<TSrc, TDest>(
            this IEnumerable<TSrc> source,
            Func<TSrc, TDest> mapFunc)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return source.Select(mapFunc).ToList();
        }

        public static PServerDto ToPServerDto(this PExternalProvider src) => new()
        {
            FirmaMandant = src.FirmaMandant,
            PServerMc = src.PServerMc,
            Key = src.Key,
            Value = src.Value,
            AendDatum = src.AendDatum,
            AendLogin = src.AendLogin,
            AendMandant = src.AendMandant,
            Name2 = src.Key
        };

        // TODO: pass-through until the provider contract is known; map typed properties
        // and mask sensitive fields (PIN, card number) here once it is.
        public static GiftCardDetailDto ToGiftCardDetailDto(this ExternalGiftCardResponse src) => new()
        {
            Fields = new Dictionary<string, System.Text.Json.JsonElement>(src.Fields)
        };

        /// <summary>
        /// Maps a validated / normalized redemption request to the provider request.
        /// </summary>
        public static ExternalRedemptionRequest ToExternalRedemptionRequest(this RedemptionRequestDto src) => new()
        {
            Id = src.Id ?? string.Empty,
            Amount = src.Amount ?? 0m,
            Currency = src.Currency ?? string.Empty,
            ReferenceNumber = src.ReferenceNumber ?? string.Empty
        };

        /// <summary>
        /// Maps the provider's redemption response. Fields the provider omits fall back to the
        /// (normalized) request values; a missing message becomes "Request successful".
        /// </summary>
        public static RedemptionResponseDto ToRedemptionResponseDto(this ExternalRedemptionResponse src, RedemptionRequestDto request) => new()
        {
            Message = string.IsNullOrWhiteSpace(src.Message) ? "Request successful" : src.Message,
            TransactionId = src.TransactionId,
            Id = string.IsNullOrWhiteSpace(src.Id) ? request.Id ?? string.Empty : src.Id,
            Amount = src.Amount ?? request.Amount ?? 0m,
            Currency = string.IsNullOrWhiteSpace(src.Currency) ? request.Currency ?? string.Empty : src.Currency,
            ReferenceNumber = string.IsNullOrWhiteSpace(src.ReferenceNumber) ? request.ReferenceNumber ?? string.Empty : src.ReferenceNumber
        };
    }
}
