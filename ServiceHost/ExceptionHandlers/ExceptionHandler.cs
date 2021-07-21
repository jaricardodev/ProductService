using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Model.Exceptions;

namespace ServiceHost.ExceptionHandlers
{
    public static class ExceptionHandler
    {
        public static RequestDelegate HandleExceptionRequest()
        {
            return async context =>
            {
                var handler = context.Features.Get<IExceptionHandlerFeature>();
                var exception = handler?.Error;

                var statusCode = (int) HttpStatusCode.InternalServerError;

                if (exception is RestException restException)
                {
                    statusCode = (int)restException.StatusCode;
                    var responseMessage = $"Error {restException.Id}: {restException.ExternalMessage}";
                    var responseJson = JsonSerializer.Serialize(new {Message = responseMessage, Code = statusCode});

                    context.Response.StatusCode = statusCode;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(responseJson);

                    return;
                }

                context.Response.StatusCode = statusCode;
            };
        }
    }
}