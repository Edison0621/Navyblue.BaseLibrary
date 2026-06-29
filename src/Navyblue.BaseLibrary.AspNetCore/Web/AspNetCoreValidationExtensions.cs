using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Navyblue.BaseLibrary.Application;

namespace Navyblue.BaseLibrary.AspNetCore;

public static class AspNetCoreValidationExtensions
{
    public static IMvcBuilder AddNavyblueApiBehavior(this IMvcBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors.Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Invalid value." : e.ErrorMessage).ToArray());

                var traceId = context.HttpContext.TraceIdentifier;
                var result = ApiResult.Fail(
                    BusinessCode.ValidationError,
                    "Request validation failed.",
                    traceId,
                    new ErrorInfo(BusinessCode.ValidationError.ToString(), "Request validation failed.", errors));

                return new BadRequestObjectResult(result);
            };
        });

        return builder;
    }
}

public static class HttpContextExtensions
{
    public static string GetTraceId(this Microsoft.AspNetCore.Http.HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Response.Headers.TryGetValue("X-Trace-Id", out var value) && !string.IsNullOrWhiteSpace(value)
            ? value.ToString()
            : context.TraceIdentifier;
    }

    public static string? GetTenantId(this Microsoft.AspNetCore.Http.HttpContext context, string headerName = "X-Tenant-Id")
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Request.Headers[headerName].FirstOrDefault() ?? context.User.FindFirst("tenant_id")?.Value;
    }
}
