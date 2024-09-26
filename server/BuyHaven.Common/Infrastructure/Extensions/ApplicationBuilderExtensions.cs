namespace BuyHaven.Common.Infrastructure.Extensions
{
    using BuyHaven.Common.Services;
    using Microsoft.EntityFrameworkCore;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebService(this IApplicationBuilder app, IWebHostEnvironment env, bool withDefaultHealthChecks = true)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseRouting()
                .UseCors(options => options
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod())
                .UseAuthentication()
                .UseAuthorization()
                .UseSwaggerUI()
                .UseEndpoints(endpoints =>
                {
                    //if (withDefaultHealthChecks)
                    //{
                    //    endpoints.MapHealthChecks(InfrastructureConstants.ConfigurationConstants.HealthCheckUrl, new HealthCheckOptions
                    //    {
                    //        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    //    });
                    //}
                    endpoints.MapControllers();
                });

            return app;
        }

        public static IApplicationBuilder UseSwaggerUI(this IApplicationBuilder app)
            => app
                .UseSwagger()
                .UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint(InfrastructureConstants.SwaggerConstants.SwaggerEndpoint, InfrastructureConstants.SwaggerConstants.SwaggerName);
                    options.RoutePrefix = string.Empty;
                });

        public static IApplicationBuilder Initialize(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var db = serviceProvider.GetRequiredService<DbContext>();

            db.Database.Migrate();

            var seeders = serviceProvider.GetServices<IDataSeeder>();

            foreach (var seeder in seeders)
            {
                seeder.SeedData();
            }

            return app;
        }

        public static IApplicationBuilder UseJwtHeaderAuthentication(this IApplicationBuilder app)
           => app
               .UseMiddleware<JwtHeaderAuthenticationMiddleware>()
               .UseAuthentication();
    }
}
