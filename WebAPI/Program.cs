using Serilog;
using WebAPI.Endpoints;
using WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args)
    .ConfigureBuilder();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDistributedMemoryCache();


builder.Configuration
     .SetBasePath(Directory.GetCurrentDirectory())
     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
     .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);


var app = builder
    .Build()
    .ConfigureApplication();

app.MapArticleEndpoints();

try
{
    Log.Information("Starting host");
    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

