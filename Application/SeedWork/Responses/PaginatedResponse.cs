namespace Application.SeedWork.Responses;

public class PaginatedResponse<T> : BaseResponse<T> where T : class
{
    //TODO: Complete pagination, with Total, Page and PageSize
    public PaginatedResponse(int statusCode, List<T> data)
    {
        StatusCode = statusCode;
        Data = data;
    }

    public PaginatedResponse(List<T> data, int page, int pageSize, int total)
    {
        StatusCode = 200;
        Data = data;
        Page = page;
        PageSize = pageSize;
        Total = total;
    }
    public PaginatedResponse(List<T> data)
    {
        StatusCode = 200;
        Data = data;
    }

    public List<T> Data { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
}