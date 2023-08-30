using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Event.Application.Filters
{
    public class SwaggerAuthMiddleware
    {
        private readonly RequestDelegate next;

        public SwaggerAuthMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                string authHeader = context.Request.Headers["Authorization"];

                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    //get the credentials from request header
                    var header = AuthenticationHeaderValue.Parse(authHeader);
                    var inBytes = Convert.FromBase64String(header.Parameter);
                    var credentials = Encoding.UTF8.GetString(inBytes).Split(':');
                    var username = credentials[0];
                    var password = credentials[1];

                    //validate credentials
                    string envCredentials = Environment.GetEnvironmentVariable("swagger_credentials");
                    string envUsername = envCredentials.Split(':')[0];
                    string envPassword = envCredentials.Split(':')[1];

                    if (username.Equals(envUsername) && password.Equals(envPassword))
                    {
                        await next.Invoke(context).ConfigureAwait(false);
                        return;
                    }
                }

                context.Response.Headers["WWW-Authenticate"] = "Basic";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                await next.Invoke(context).ConfigureAwait(false);
            }
        }

    }

    public static class SwaggerAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseSwaggerAuthorized(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SwaggerAuthMiddleware>();
        }
    }
}
