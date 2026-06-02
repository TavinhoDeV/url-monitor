using Microsoft.EntityFrameworkCore;
using UrlMonitor.Data;
using UrlMonitor.Services;

namespace UrlMonitor;

public class MonitorWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MonitorWorker> _logger;

    // Rastreia quando cada URL foi verificada pela última vez
    private readonly Dictionary<int, DateTime> _ultimaVerificacao = new();

    public MonitorWorker(IServiceScopeFactory scopeFactory, ILogger<MonitorWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 URL Monitor iniciado em {Hora}", DateTime.Now);

        // Inicializa o banco
        using (var scope = _scopeFactory.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync(stoppingToken);
            _logger.LogInformation("✅ Banco de dados pronto.");
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await VerificarUrlsAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task VerificarUrlsAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var checker = scope.ServiceProvider.GetRequiredService<HttpCheckerService>();
        var incidentService = scope.ServiceProvider.GetRequiredService<IncidentService>();

        var targets = await db.UrlTargets
            .Where(t => t.Ativo)
            .ToListAsync(ct);

        var agora = DateTime.UtcNow;
        var tarefas = new List<Task>();

        foreach (var target in targets)
        {
            var deveVerificar = !_ultimaVerificacao.TryGetValue(target.Id, out var ultima)
                || (agora - ultima).TotalSeconds >= target.IntervalSeconds;

            if (!deveVerificar) continue;

            tarefas.Add(Task.Run(async () =>
            {
                var resultado = await checker.CheckAsync(target);

                using var innerScope = _scopeFactory.CreateScope();
                var innerDb = innerScope.ServiceProvider.GetRequiredService<AppDbContext>();
                var innerIncident = innerScope.ServiceProvider.GetRequiredService<IncidentService>();

                innerDb.CheckResults.Add(resultado);
                await innerDb.SaveChangesAsync(ct);
                await innerIncident.ProcessarAsync(resultado);
            }, ct));

            _ultimaVerificacao[target.Id] = agora;
        }

        if (tarefas.Any())
            await Task.WhenAll(tarefas);
    }
}
