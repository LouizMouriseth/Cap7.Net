using Application.SeedWork.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.SeedWork.Filters;

public class BaseResponseResultFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    /// <summary>
    /// Sets Status code after controller action
    /// </summary>
    /// <param name="context">Context of the executed action</param>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is not ObjectResult objectResult || objectResult.Value == null) return;
        
        var type = objectResult.Value!.GetType();

        if (!type.IsGenericType || 
            type.BaseType == null || 
            type.BaseType!.GetGenericTypeDefinition() != typeof(BaseResponse<>)) return;
        
        var method = type.GetMethod(nameof(BaseResponse<object>.GetStatusCode))!;
            
        context.Result = new ObjectResult(objectResult.Value)
        {
            StatusCode = (int)method.Invoke(objectResult.Value, null)!
        };
        
    }
}