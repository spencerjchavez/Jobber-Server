using Jobber_Server.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

class ExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, 
        Exception exception, 
        CancellationToken cancellation)
    {
        
        context.Response.StatusCode = exception switch
        {
            NotFoundException => 404,
            BadRequestException => 400,
            UnauthorizedException => 401,
            _ => 500
        };

        context.Response.ContentType = "application/json";
        var error = new { message = exception.Message };
        await context.Response.WriteAsJsonAsync(error, cancellation);

        return true;
    }
}