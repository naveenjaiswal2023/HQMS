using AuthService.Functions.Middleware;

public class Program
{
    public static void Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults(worker =>
            {
                worker.UseMiddleware<ExceptionHandlingMiddleware>(); // Optional: global exception logging
            })
            .ConfigureServices(services =>
            {
                services.AddLogging(); // 🔥 Logging support
                // services.AddScoped<IAuthService, AuthService>(); // Your DI
            })
            .Build();

        host.Run();
    }
}
