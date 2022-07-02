using my_books.Data.ViewModels;
using System.Net;

namespace my_books.Exceptions
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {

                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ErrorVM()
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error from the custom middleware",
                Path = "path"
            };

            return context.Response.WriteAsync(response.ToString());
        }
    }
}
