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
    }
}
