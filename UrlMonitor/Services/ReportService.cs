using Microsoft.EntityFrameworkCore;
using UrlMonitor.Data;
using UrlMonitor.Models;

namespace UrlMonitor.Services;

public class ReportService
{
    private readonly AppDbContext _db;

    public ReportService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<object> GerarResumoAsync()
    {
        var targets = await _db.UrlTargets.Where(t => t.Ativo).ToListAsync();
        var agora = DateTime.UtcNow;
        var ultimaHora = agora.AddHours(-1);

        var resultados = await _db.CheckResults
            .Where(r => r.VerificadoEm >= ultimaHora)
            .ToListAsync();

        var incidentesAbertos = await _db.Incidents
            .Where(i => i.ResolvidoEm == null)
            .CountAsync();

        var resumo = targets.Select(t =>
        {
            var checks = resultados.Where(r => r.UrlTargetId == t.Id).ToList();
            var online = checks.Count(r => r.Status == CheckStatus.Online);
            var total = checks.Count;
            var uptime = total > 0 ? (double)online / total * 100 : 100;
            var avgMs = checks.Any() ? checks.Average(r => r.ResponseTimeMs) : 0;

            return new
            {
                t.Id,
                t.Nome,
                t.Url,
                UptimePct = Math.Round(uptime, 1),
                ChecagensUltimaHora = total,
                MediaRespostaMs = Math.Round(avgMs, 0),
                UltimoStatus = checks.OrderByDescending(r => r.VerificadoEm).FirstOrDefault()?.Status.ToString() ?? "Sem dados"
            };
        });

        return new
        {
            GeradoEm = agora,
            TotalUrls = targets.Count,
            IncidentesAbertos = incidentesAbertos,
            Urls = resumo
        };
    }
}
