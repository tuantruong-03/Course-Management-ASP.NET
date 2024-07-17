using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Middlewares;

namespace api.Extensions
{
    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}