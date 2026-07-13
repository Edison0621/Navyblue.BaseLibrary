using System.Threading.RateLimiting;
using NavyblueWebApi.Application.Authentication;
using NavyblueWebApi.Application.Users.Commands;
using NavyblueWebApi.Domain.Authentication;
using NavyblueWebApi.Domain.Users;
using NavyblueWebApi.Infrastructure;
using NavyblueWebApi.Infrastructure.Caching;
using NavyblueWebApi.Infrastructure.Persistence;
using NavyblueWebApi.Web.Authentication;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Navyblue.Foundation.AspNetCore;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.DependencyInjection;
using Navyblue.Foundation.Security;
using Serilog;

namespace NavyblueWebApi.Web;

public sealed class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console());

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
                {
                    if (builder.Environment.IsDevelopment())
                        options.SigningKey = "Navyblue-Dev-Only-Signing-Key-Change-Me-32+";
                    else
                        throw new InvalidOperationException(
                            "Jwt:SigningKey is required. Set it via environment variable Jwt__SigningKey or user-secrets.");
                }

                if (builder.Environment.IsDevelopment())
                    options.RequireHttpsMetadata = false;
            });

            builder.Services.AddNavyblueCqrs(typeof(AddUserCommand).Assembly);
            builder.Services.AddInfrastructure(configuration);
            builder.Services.AddSingleton<ITokenIssuer, JwtTokenIssuer>();
            builder.Services.AddAuthorization();

            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddFixedWindowLimiter("auth", limiter =>
                {
                    limiter.PermitLimit = 20;
                    limiter.Window = TimeSpan.FromMinutes(1);
                    limiter.QueueLimit = 0;
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            });

            builder.Services.AddHealthChecks()
                .AddDbContextCheck<AppDbContext>("mysql", failureStatus: HealthStatus.Unhealthy, tags: ["ready"])
                .AddCheck<RedisHealthCheck>("redis", failureStatus: HealthStatus.Unhealthy, tags: ["ready"]);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "NavyblueWebApi API",
                    Version = "v1",
                    Description = "DDD/CQRS Web API on Navyblue Foundation (JWT + ApiResult + EF Core / MySQL 8.0 + Redis)."
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
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "NavyblueWebApi v1");
                    options.RoutePrefix = "swagger";
                });
            }

            app.UseSerilogRequestLogging();
            app.UseNavyblueFramework();
            app.UseCors();
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHealthChecks("/health");
            app.MapNavybluePing();

            using (IServiceScope scope = app.Services.CreateScope())
            {
                AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.MigrateAsync();
                await EnsureDemoAdminAsync(db);
            }

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly.");
            throw;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    /// <summary>
    ///     Hard-coded demo admin for local runs. Skip if any user already exists.
    /// </summary>
    private static async Task EnsureDemoAdminAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Users.AnyAsync(cancellationToken).ConfigureAwait(false))
            return;

        const string email = "admin@navyblue.local";
        const string password = "Admin@123";
        string hash = PasswordHasher.Hash(password);
        string salt = hash.Split('$') is { Length: 4 } parts ? parts[2] : string.Empty;

        User admin = new(1L, "Administrator", email, createdBy: "seed");
        Auth auth = new(2L, admin.Id, email, hash, salt);

        db.Users.Add(admin);
        db.Auths.Add(auth);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
