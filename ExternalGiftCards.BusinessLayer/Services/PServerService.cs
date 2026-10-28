using ExternalGiftCards.BusinessLayer.Common;
using ExternalGiftCards.BusinessLayer.Interfaces;
using ExternalGiftCards.BusinessLayer.MappingProfile;
using ExternalGiftCards.BusinessLayer.Models;
using ExternalGiftCards.Repositories.Interfaces;

namespace ExternalGiftCards.BusinessLayer.Services
{
    public class PServerService : IPServerService
    {
        private readonly IPServerRepository _pServerRepository;

        public PServerService(IPServerRepository pServerRepository)
        {
            _pServerRepository = pServerRepository;
        }
        public async Task<string> GetPServerValueAsync(string firma, string mc, string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(firma) || string.IsNullOrWhiteSpace(mc) || string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("'firma', 'mc' and 'key' are required.");
            }
            var providerValue = await _pServerRepository.GetPServerAsync(firma.Trim(), mc.Trim(), key.Trim(), cancellationToken);
            if (providerValue is null)
            {
                throw new KeyNotFoundException($"No external provider entry found for firma '{firma}', mc '{mc}' and key '{key}'.");
            }
            return providerValue.ToPServerDto().Value;
        }
        public async Task<IServiceResponse<PServerDto>> GetPServerAsync(string firma, string mc, string key, CancellationToken cancellationToken = default)
        {
            var response = new ServiceResponse<PServerDto>();

            if (string.IsNullOrWhiteSpace(firma) || string.IsNullOrWhiteSpace(mc) || string.IsNullOrWhiteSpace(key))
            {
                response.Status = ResponseStatus.BadRequest;
                response.ErrorMessage = "'firma', 'mc' and 'key' are required.";
                return response;
            }

            var providerValue = await _pServerRepository.GetPServerAsync(firma.Trim(), mc.Trim(), key.Trim(), cancellationToken);
            if (providerValue is null)
            {
                response.Status = ResponseStatus.NotFound;
                response.ErrorMessage = $"No external provider entry found for firma '{firma}', mc '{mc}' and key '{key}'.";
                return response;
            }

            response.Result = providerValue.ToPServerDto();
            return response;
        }
    }
}
