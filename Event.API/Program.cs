using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Event.Application.DataContext;
using Event.Data.Context;


namespace Event.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            await ApplyMigrations(host.Services);

            await host.RunAsync();
        }
        private static async Task ApplyMigrations(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            await using var dbcontext1 = scope.ServiceProvider.GetService<EventDbContext>();
            await using var dbcontext2 = scope.ServiceProvider.GetService<ApplicationDbContext>();

            await dbcontext2.Database.MigrateAsync();
            await dbcontext1.Database.MigrateAsync();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
