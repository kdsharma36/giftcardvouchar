namespace ExternalGiftCards.BusinessLayer.Common
{
    public enum ResponseStatus
    {
        Ok,
        BadRequest,
        NotFound,
        NoContent,
        InvalidPropertyValues,
        Unauthorized,
        Forbidden,
        BadGateway
    }

    public interface IServiceResponse<T>
    {
        string ErrorMessage { get; set; }
        T Result { get; set; }
        ResponseStatus Status { get; set; }
    }
}
