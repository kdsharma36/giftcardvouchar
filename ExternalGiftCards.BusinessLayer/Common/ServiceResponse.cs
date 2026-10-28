namespace ExternalGiftCards.BusinessLayer.Common
{
    public class ServiceResponse<T> : IServiceResponse<T>
    {
        public string ErrorMessage { get; set; } = null!;
        public T Result { get; set; } = default!;
        public ResponseStatus Status { get; set; } = ResponseStatus.Ok;
    }
}
