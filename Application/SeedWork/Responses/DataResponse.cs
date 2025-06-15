namespace Application.SeedWork.Responses;

public class DataResponse<T> : BaseResponse<T> where T : class
{
    public DataResponse(int statusCode, string message, T? data) : base(message)
    {
        StatusCode = statusCode;
        Data = data;
    }

    public DataResponse(string message, T? data) : base(message)
    {
        StatusCode = 200;
        Data = data;
    }
    
    public T? Data { get; private set; }
}