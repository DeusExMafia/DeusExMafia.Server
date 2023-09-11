namespace DeusExMafia.Server.Middleware;

static class GameplayMiddlewareExtensions {
    public static IApplicationBuilder UseGameMiddleware(this IApplicationBuilder app) {
        return app.UseMiddleware<GameplayMiddleware>();
    }
}
