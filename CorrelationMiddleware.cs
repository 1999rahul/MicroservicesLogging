using SerilogCorealtionIdLoggingDemo;

namespace SerilogCorrelationMiddleware;
public class CorrelationMiddleware
{
    private readonly RequestDelegate _next;

    private const string _correlationIdHeader = "X-CorrelationId";
    public CorrelationMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context,ICorrelationIdGenerator correlationIdGenerator)
    {
        var correlationId = GetCorrelationIdTrace(context, correlationIdGenerator);
        AddCorrelationIdToResponse(context, correlationId);
        var logger = context.RequestServices.GetRequiredService<ILogger<CorrelationMiddleware>>();

        using (logger.BeginScope("{@CorrelationId}", correlationId))
        {
            await _next(context);
        }
    }

    private static string GetCorrelationIdTrace(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
    {
        if(context.Request.Headers.TryGetValue(_correlationIdHeader,out var correlationId))
        {
            correlationIdGenerator.Set(correlationId);
            return correlationId;
        }
        else
        {
            return correlationIdGenerator.Get();
        }
    }

    private static void AddCorrelationIdToResponse(HttpContext context, string correlationId)
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Append(_correlationIdHeader, new[] { correlationId });
            return Task.CompletedTask;
        });
    }
}

