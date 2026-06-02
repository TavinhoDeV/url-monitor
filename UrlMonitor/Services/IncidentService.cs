using Microsoft.EntityFrameworkCore;
using UrlMonitor.Data;
using UrlMonitor.Models;

namespace UrlMonitor.Services;

public class IncidentService
{
    private readonly AppDbContext _db;
    private readonly ILogger<IncidentService> _logger;

    public IncidentService(AppDbContext db, ILogger<IncidentService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task ProcessarAsync(CheckResult resultado)
    {
        var incidenteAberto = await _db.Incidents
            .FirstOrDefaultAsync(i => i.UrlTargetId == resultado.UrlTargetId && i.ResolvidoEm == null);

        bool isDown = resultado.Status != CheckStatus.Online;

        if (isDown && incidenteAberto == null)
        {
            // Abre novo incidente
            var incidente = new Incident
            {
                UrlTargetId = resultado.UrlTargetId,
                IniciadoEm = DateTime.UtcNow,
                Motivo = resultado.Mensagem ?? resultado.Status.ToString(),
            };
            _db.Incidents.Add(incidente);
            await _db.SaveChangesAsync();

            _logger.LogWarning("🚨 INCIDENTE ABERTO — UrlTarget #{Id}: {Motivo}",
                resultado.UrlTargetId, incidente.Motivo);
        }
        else if (!isDown && incidenteAberto != null)
        {
            // Resolve incidente existente
            incidenteAberto.ResolvidoEm = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            _logger.LogInformation("✅ INCIDENTE RESOLVIDO — UrlTarget #{Id} — Duração: {Duracao}",
                resultado.UrlTargetId, incidenteAberto.Duracao?.ToString(@"hh\:mm\:ss"));
        }
    }

    public async Task<List<Incident>> ListarAbertosAsync()
    {
        return await _db.Incidents
            .Include(i => i.UrlTarget)
            .Where(i => i.ResolvidoEm == null)
            .OrderByDescending(i => i.IniciadoEm)
            .ToListAsync();
    }

    public async Task<List<Incident>> ListarTodosAsync(int limit = 50)
    {
        return await _db.Incidents
            .Include(i => i.UrlTarget)
            .OrderByDescending(i => i.IniciadoEm)
            .Take(limit)
            .ToListAsync();
    }
}
