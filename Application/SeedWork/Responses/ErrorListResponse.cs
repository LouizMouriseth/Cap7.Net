using FluentValidation.Results;
using Mapster;

namespace Application.SeedWork.Responses;

public class ErrorListResponse<T> : BaseResponse<T> where T : class
{
    public ErrorListResponse(string message, List<ValidationFailure> errors) : base(message)
    {
        StatusCode = 422;
        Errors = errors.Adapt<List<ErrorItem>>();
    }

    public ErrorListResponse(int statusCode, string message, List<ValidationFailure> errors) : base(message)
    {
        StatusCode = statusCode;
        Errors = errors.Adapt<List<ErrorItem>>();
    }
    
    public List<ErrorItem> Errors { get; private set; }
    
    public class ErrorItem
    {
        public string PropertyName { get; private set; }
        public string ErrorMessage { get; private set; }
    }
}