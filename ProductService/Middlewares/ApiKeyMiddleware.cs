namespace ProductService.Middlewares
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private const string ApiKeyHeader = "X-API-KEY";
        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip API key check for auth endpoints (e.g., login/register)
            if (context.Request.Path.StartsWithSegments("/api/auth", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var extractedApiKey))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsJsonAsync(new {message = "API Key is missing" });
                return;
            }
            var apiKey = _configuration.GetValue<string>("ApiSettings:ApiKey"); // This should be stored securely
            if (string.IsNullOrWhiteSpace(apiKey) || !string.Equals(apiKey, extractedApiKey, StringComparison.Ordinal))
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Unauthorized client");
                return;
            }
            await _next(context);
        }

    }

    public static class ApiKeyMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyMiddleware>();
        }
    }
}
