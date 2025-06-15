namespace Application.SeedWork.Responses;

public abstract class BaseResponse<T> where T : class
{
    public BaseResponse()
    {
        
    }
    public BaseResponse(string message)
    {
        Message = message;
    }

    protected int StatusCode { get; set; }
    public string? Message { get; private set; }
    
    public int GetStatusCode() => StatusCode;
}