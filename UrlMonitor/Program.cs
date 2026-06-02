using Microsoft.EntityFrameworkCore;
using Serilog;
using UrlMonitor;
using UrlMonitor.Data;
using UrlMonitor.Services;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/monitor-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.AddSerilog();

    // Banco de dados
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

    // HttpClient para checks
    builder.Services.AddHttpClient("monitor", client =>
    {
        client.DefaultRequestHeaders.Add("User-Agent", "UrlMonitor/1.0");
    });

    // Serviços
    builder.Services.AddScoped<HttpCheckerService>();
    builder.Services.AddScoped<IncidentService>();
    builder.Services.AddScoped<ReportService>();

    // Controllers + Static Files
    builder.Services.AddControllers();

    // Worker
    builder.Services.AddHostedService<MonitorWorker>();

    var app = builder.Build();

    // Serve o dashboard (index.html)
    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicacao encerrada inesperadamente.");
}
finally
{
    Log.CloseAndFlush();
}