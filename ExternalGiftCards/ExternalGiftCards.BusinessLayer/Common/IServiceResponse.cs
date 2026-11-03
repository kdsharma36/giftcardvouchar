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

        /// <summary>
        /// Optional business error code (messagecode) for endpoints that return the
        /// { messagecode, errormessage } contract. Null for endpoints that don't use it.
        /// </summary>
        string? ErrorCode { get; set; }
    }
}
