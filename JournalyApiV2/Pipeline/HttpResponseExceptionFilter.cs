using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JournalyApiV2.Pipeline;

public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is HttpAccessDeniedException httpAccessDeniedException)
        {
            var message = string.IsNullOrEmpty(httpAccessDeniedException.Message) || httpAccessDeniedException.Message == new HttpAccessDeniedException().Message
                ? "Access to the resource was denied"
                : httpAccessDeniedException.Message;
            context.Result = new ObjectResult(message)
            {
                StatusCode = 403
            };
            context.ExceptionHandled = true;
        }

        if (context.Exception is HttpNotFoundExeption httpNotFoundExeption)
        {
            var message = string.IsNullOrEmpty(httpNotFoundExeption.Message) || httpNotFoundExeption.Message == new HttpNotFoundExeption().Message
                ? "Resource not found"
                : httpNotFoundExeption.Message;
            context.Result = new ObjectResult(message)
            {
                StatusCode = 404
            };
            context.ExceptionHandled = true;
        }
        
        if (context.Exception is HttpBadRequestException httpBadRequestException)
        {
            var message = string.IsNullOrEmpty(httpBadRequestException.Message) || httpBadRequestException.Message == new HttpNotFoundExeption().Message
                ? "Resource not found"
                : httpBadRequestException.Message;
            context.Result = new ObjectResult(message)
            {
                StatusCode = 400
            };
            context.ExceptionHandled = true;
        }
    }
}