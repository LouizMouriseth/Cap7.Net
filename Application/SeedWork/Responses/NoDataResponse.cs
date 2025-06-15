using FluentValidation.Results;
using Mapster;

namespace Application.SeedWork.Responses;

public class NoDataResponse<T> : BaseResponse<T> where T : class
{
    public NoDataResponse(string message) : base(message)
    {
        StatusCode = 200;
    }

    public NoDataResponse(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}