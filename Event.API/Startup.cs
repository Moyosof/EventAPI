using Event.API.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using Event.Application.EmailService.Entities;
using Event.Application.Filters;
using Event.Application.SmsService.Entities;
using Event.DTO;
using Event.DTO.EmailClients;
using Event.DTO.OAuth;
using Event.IoC.Dependencies;
using static System.Net.Mime.MediaTypeNames;

namespace Event.API
{
    public class Startup
    {
        private readonly static string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        public static string[] _origins { get; set; }
        public string SpecificOrigin { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            if (env.Equals("Production", StringComparison.OrdinalIgnoreCase))
            {
                _origins = new[]
                {
                    Configuration["CorsOrigin:MobileApp"]
                };
            }

            if (env.Equals("Staging", StringComparison.OrdinalIgnoreCase))
            {
                _origins = new[]
                {
                    Configuration["CorsOrigin:MobileApp"]
                };
            }

            if (env.Equals("Development", StringComparison.OrdinalIgnoreCase))
            {
                _origins = new[]
                {
                    Configuration["CorsOrigin:MobileApp"]
                };
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.ConfigureRepositoryAndServices(Configuration);
            services.ConfigureIdentity();
            services.ConfigureDatabaseConnection(Configuration);
            services.ConfigureJWT(Configuration);
            services.ConfigureFluentEmail(Configuration);
            services.ConfigureSwagger();
            services.ConfigureApiVersioning();
            services.ConfigureCORS(Configuration);
            services.ConfigureModelState();
            SpecificOrigin = services.ConfigureCorsSpecificOrigin(_origins);


            services.AddMvc().AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            #region CONFIGURE SERVICES
            services.Configure<DataProtectionTokenProviderOptions>(opt => { opt.TokenLifespan = TimeSpan.FromHours(6); });
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            services.Configure<Application.EmailService.Entities.SendGridKey>(Configuration.GetSection("SendGridKey"));
            services.Configure<Application.EmailService.Entities.SendGridFrom>(Configuration.GetSection("SendGridFrom"));
            services.Configure<Application.EmailService.Entities.GmailOptions>(Configuration.GetSection("GmailOptions"));
            services.Configure<ConnectionStringDTO>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<BulkGateSMS>(Configuration.GetSection("BulkGateAPI"));
            services.Configure<HollaTagsSMS>(Configuration.GetSection("HollaTagsAPI"));
            #endregion



            #region Authorization Filter
            services.AddControllers(opt =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

                opt.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();
            #endregion

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

            services.AddRouting(opt =>
            {
                opt.LowercaseUrls = true;
                opt.LowercaseQueryStrings = false;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Selenia.API v1"));
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseCors(SpecificOrigin);

            app.UseAuthorization();
            app.ConfigureExceptionHandler();

            app.UseSwaggerAuthorized();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event.API v1"));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
