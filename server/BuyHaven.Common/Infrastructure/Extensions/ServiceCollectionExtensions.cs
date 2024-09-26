namespace BuyHaven.Common.Infrastructure.Extensions
{
    using BuyHaven.Common.Messaging;
    using BuyHaven.Common.Services.Identity;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using System.Reflection;
    using System.Text;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebService<TDbContext>(this IServiceCollection services, IConfiguration configuration)
            where TDbContext : DbContext
        {
            services
                .AddDatabase<TDbContext>(configuration)
                .AddApplicationSettings(configuration)
                .AddTokenAuthentication(configuration)
                .AddAutoMapperProfile(Assembly.GetCallingAssembly())
                .AddSwagger()
                .AddHealthCheck(configuration)
                .AddControllers();

            return services;
        }

        public static IServiceCollection AddApplicationSettings(this IServiceCollection services, IConfiguration configuration)
            => services
                .Configure<AppSettings>(configuration
                    .GetSection(nameof(AppSettings)));

        public static IServiceCollection AddDatabase<TDbContext>(this IServiceCollection services, IConfiguration configuration)
            where TDbContext : DbContext
           => services
               .AddScoped<DbContext, TDbContext>()
               .AddDbContext<TDbContext>(options => options
                   .UseSqlServer(
                            configuration.GetConnectionString(InfrastructureConstants.ConfigurationConstants.DefaultConnectionString),
                            sqlOpts =>
                            {
                                sqlOpts.EnableRetryOnFailure(
                                        maxRetryCount: InfrastructureConstants.ConfigurationConstants.DefaultMaxRetryCount,
                                        maxRetryDelay: TimeSpan.FromSeconds(InfrastructureConstants.ConfigurationConstants.DefaultMaxTimeoutInSec),
                                        errorNumbersToAdd: null);
                            }));

        public static IServiceCollection AddTokenAuthentication(
            this IServiceCollection services,
            IConfiguration configuration,
            JwtBearerEvents events = null)
        {
            var secret = configuration
                .GetSection(nameof(AppSettings))
                .GetValue<string>(nameof(AppSettings.Secret));

            var key = Encoding.ASCII.GetBytes(secret);

            services
                .AddAuthentication(authentication =>
                {
                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(bearer =>
                {
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };

                    if (events != null)
                    {
                        bearer.Events = events;
                    }
                });

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
           => services.AddSwaggerGen(c =>
           {
               c.SwaggerDoc(
                   "v1",
                   new OpenApiInfo
                   {
                       Title = Assembly.GetExecutingAssembly().GetName().Name,
                       Version = "v1"
                   });
           });

        public static IServiceCollection AddAutoMapperProfile(this IServiceCollection services, Assembly assembly)
            => services
                .AddAutoMapper(
                    (_, config) => config
                        .AddProfile(new MappingProfile(assembly)),
                    Array.Empty<Assembly>());

        public static IServiceCollection AddHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var healthChecks = services.AddHealthChecks();
            var appSettings = configuration.GetSection(nameof(AppSettings));
            var rabbitHost = appSettings.GetValue<string>(nameof(AppSettings.RabbitHost));
            var rabbitUser = appSettings.GetValue<string>(nameof(AppSettings.RabbitUsername));
            var rabbitPass = appSettings.GetValue<string>(nameof(AppSettings.RabbitPassword));
            var rabbitConnection = $"amqp://{rabbitHost}:{rabbitUser}@{rabbitPass}/";
            healthChecks
                .AddSqlServer(configuration.GetConnectionString(InfrastructureConstants.ConfigurationConstants.DefaultConnectionString));
                //.AddRabbitMQ(rabbitConnectionString: rabbitConnection);

            return services;
        }
    }
}
