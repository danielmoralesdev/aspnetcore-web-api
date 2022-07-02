﻿using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using my_books.Data.ViewModels;
using System.Net;

namespace my_books.Exceptions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureBuildExceptionHandler(this IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandler(appError =>
            appError.Run(async context =>
            {
                var logger = loggerFactory.CreateLogger("ConfigureBuildExceptionHandler");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                var contextRequest = context.Features.Get<IHttpRequestFeature>();

                if (contextFeature != null)
                {
                    var errorVMString = new ErrorVM()
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = contextFeature.Error.Message,
                        Path = contextRequest.Path
                    }.ToString();

                    logger.LogError(errorVMString);

                    await context.Response.WriteAsync(errorVMString);
                }
            }));
        }

        public static void ConfigureCustomExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomExceptionMiddleware>();
        }
    }
}
