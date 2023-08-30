using Microsoft.AspNetCore.Builder;

namespace Event.API.Filters
{
    public static class GlobalExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            //app.UseExceptionHandler(appError =>
            //{
            //    appError.Run(async context =>
            //    {
            //        context.Response.StatusCode = (int)HttpStatusCode.OK;
            //        context.Response.ContentType = "application/json";

            //        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
            //        if (contextFeature is not null)
            //        {
            //            //log error
            //            var services = app.ApplicationServices;
            //            var sql = services.GetService<ISqlDBObjects>();
            //            await sql.StoredProcedures.LogSystemError(contextFeature.Error.Message, $"{contextFeature.Path}:{contextFeature.RouteValues}");

            //            await context.Response.WriteAsJsonAsync(new JsonMessage<string>()
            //            {
            //                Status = false,
            //                StatusCode = (int)HttpStatusCode.InternalServerError,
            //                ErrorMessage = ResponseMessages.InternalServerError
            //            });
            //        }
            //    });
            //});
        }

    }
}
