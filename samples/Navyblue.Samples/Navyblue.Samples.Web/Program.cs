using Navyblue.Samples.Application.Authentication;
using Navyblue.Samples.Application.Users;
using Navyblue.Samples.Application.Users.Commands;
using Navyblue.Samples.Infrastructure;
using Navyblue.Samples.Web.Authentication;
using Microsoft.OpenApi.Models;
using Navyblue.Foundation.AspNetCore;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.DependencyInjection;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Samples.Web;

public sealed class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IConfiguration configuration = builder.Configuration;

        builder.Services.AddNavyblueFoundation(options =>
        {
            options.WorkerId = configuration.GetValue("Navyblue:WorkerId", 1);
            options.DataCenterId = configuration.GetValue("Navyblue:DataCenterId", 1);
            options.RegisterInMemoryEventBus = true;
        });

        builder.Services.AddNavyblueFramework(options =>
        {
            options.EnableTraceId = true;
            options.EnableExceptionHandling = true;
            options.EnableRequestLogging = true;
            options.EnableSecurityHeaders = true;
            options.EnableTenantResolution = false;
            options.EnableRequestContext = true;
            options.EnableAuditLogging = false;
            options.WrapApiResult = false;
        });

        builder.Services.AddControllers().AddNavyblueApiBehavior();

        builder.Services.AddNavyblueJwt(options =>
        {
            configuration.GetSection("Jwt").Bind(options);
            if (string.IsNullOrWhiteSpace(options.SigningKey))
                options.SigningKey = "Navyblue-Samples-Signing-SuperSecret-Signing-Key-32+";
            options.RequireHttpsMetadata = false;
        });

        builder.Services.AddNavyblueCqrs(typeof(AddUserCommand).Assembly);
        builder.Services.AddInfrastructure();
        builder.Services.AddSingleton<ITokenIssuer, JwtTokenIssuer>();
        builder.Services.AddAuthorization();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
                policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        });

        builder.Services.AddHealthChecks();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Navyblue.Samples API",
                Version = "v1",
                Description = "DDD/CQRS sample on Navyblue Foundation (JWT + ApiResult + in-memory repos)."
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste JWT from POST /api/auth/login (without the 'Bearer ' prefix)."
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        WebApplication app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Navyblue.Samples v1");
                options.RoutePrefix = "swagger";
            });
        }

        app.UseNavyblueFramework();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/health");
        app.MapNavybluePing();

        using (IServiceScope scope = app.Services.CreateScope())
        {
            IServiceProvider sp = scope.ServiceProvider;
            await DataSeeder.SeedAsync(
                sp.GetRequiredService<IUserRepository>(),
                sp.GetRequiredService<IAuthRepository>(),
                sp.GetRequiredService<IIdGenerator<long>>());
        }

        app.Run();
    }
}
