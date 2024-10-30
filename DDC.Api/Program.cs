using System.Text.Json;
using System.Text.Json.Serialization;
using DDC.Api;
using DDC.Api.Exceptions;
using DDC.Api.Repositories;
using DDC.Api.Workers;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

#if DEBUG
const LogEventLevel defaultLoggingLevel = LogEventLevel.Debug;
#else
const LogEventLevel defaultLoggingLevel = LogEventLevel.Information;
#endif
const LogEventLevel infrastructureLoggingLevel = LogEventLevel.Information;
const string outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} ({SourceContext}){NewLine}{Exception}";
Log.Logger = new LoggerConfiguration().WriteTo.Console(outputTemplate: outputTemplate).Enrich.WithProperty("SourceContext", "Bootstrap").CreateBootstrapLogger();

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog(
        opt =>
        {
            opt.WriteTo.Console(outputTemplate: outputTemplate)
                .Enrich.WithProperty("SourceContext", "Bootstrap")
                .MinimumLevel.Is(defaultLoggingLevel)
                .MinimumLevel.Override("System.Net.Http.HttpClient", infrastructureLoggingLevel)
                .MinimumLevel.Override("Microsoft.Extensions.Http", infrastructureLoggingLevel)
                .MinimumLevel.Override("Microsoft.AspNetCore", infrastructureLoggingLevel)
                .MinimumLevel.Override("Microsoft.Identity", infrastructureLoggingLevel)
                .MinimumLevel.Override("Microsoft.IdentityModel", infrastructureLoggingLevel)
                .ReadFrom.Configuration(builder.Configuration);
        }
    );

    builder.Services.AddControllers()
        .AddJsonOptions(
            options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }
        );
    builder.Services.AddControllersWithViews();
    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<ExceptionHandler>();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddOpenApiDocument(
        settings =>
        {
            settings.Title = "Dofus Data Center - API";
            settings.Description = "Data extracted from Dofus.";
            settings.Version = Metadata.Version?.ToString() ?? "~dev";
        }
    );

    builder.Services.AddHttpClient();

    builder.Services.AddSingleton<RawDataFromGithubReleasesSavedToDisk>(
        services => new RawDataFromGithubReleasesSavedToDisk(
            services.GetRequiredService<IOptions<RepositoryOptions>>(),
            services.GetRequiredService<ILogger<RawDataFromGithubReleasesSavedToDisk>>()
        )
    );
    builder.Services.AddSingleton<IRawDataRepository, RawDataFromGithubReleasesSavedToDisk>(services => services.GetRequiredService<RawDataFromGithubReleasesSavedToDisk>());

    builder.Services.AddHostedService<DownloadDataFromGithubReleases>();

    builder.Services.Configure<RepositoryOptions>(
        o =>
        {
            string? repositoryPath = builder.Configuration.GetValue<string?>("RepositoryPath", null);
            if (!string.IsNullOrWhiteSpace(repositoryPath))
            {
                o.BasePath = Path.GetFullPath(Environment.ExpandEnvironmentVariables(repositoryPath));
            }
        }
    );

    WebApplication app = builder.Build();

    ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();

    string? pathBase = app.Configuration.GetValue<string>("PathBase");
    if (!string.IsNullOrWhiteSpace(pathBase))
    {
        logger.LogInformation("Using path base '{PathBase}'.", pathBase);
        app.UsePathBase(pathBase);
    }

    app.UseExceptionHandler();

    app.UseOpenApi();
    app.UseSwaggerUi();

    app.MapDefaultControllerRoute();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}