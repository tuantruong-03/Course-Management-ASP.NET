using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using api.Exceptions;
using Newtonsoft.Json;

namespace api.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException appEx)
            {
                await HandleAppExceptionAsync(context, appEx);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleAppExceptionAsync(HttpContext context, AppException exception)
        {
            var response = new
            {
                statusCode = exception.StatusCode,
                message = exception.Message
            };

            var payload = JsonConvert.SerializeObject(response);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception.StatusCode;
            return context.Response.WriteAsync(payload);
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = new
            {

                message = exception.Message
            };

            var payload = JsonConvert.SerializeObject(response);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(payload);
        }
    }
}