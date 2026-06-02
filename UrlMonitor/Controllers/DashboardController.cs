using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlMonitor.Data;
using UrlMonitor.Models;

namespace UrlMonitor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("status")]
    public async Task<IActionResult> Status()
    {
        var targets = await _db.UrlTargets.Where(t => t.Ativo).OrderBy(t => t.Nome).ToListAsync();
        var ultimaHora = DateTime.UtcNow.AddHours(-1);
        var resultado = new List<object>();

        foreach (var t in targets)
        {
            var checks = await _db.CheckResults
                .Where(r => r.UrlTargetId == t.Id)
                .OrderByDescending(r => r.VerificadoEm)
                .Take(20)
                .ToListAsync();

            var ultimo = checks.FirstOrDefault();
            var checksHora = checks.Where(r => r.VerificadoEm >= ultimaHora).ToList();
            var online = checksHora.Count(r => r.Status == CheckStatus.Online);
            var total = checksHora.Count;
            var uptime = total > 0 ? Math.Round((double)online / total * 100, 1) : 100.0;
            var avgMs = checks.Any() ? Math.Round(checks.Average(r => r.ResponseTimeMs), 0) : 0;

            var incidenteAberto = await _db.Incidents
                .Where(i => i.UrlTargetId == t.Id && i.ResolvidoEm == null)
                .FirstOrDefaultAsync();

            resultado.Add(new
            {
                t.Id,
                t.Nome,
                t.Url,
                Status = ultimo?.Status.ToString() ?? "Sem dados",
                StatusCode = ultimo?.StatusCode,
                ResponseTimeMs = ultimo?.ResponseTimeMs ?? 0,
                MediaRespostaMs = avgMs,
                UptimePct = uptime,
                UltimaVerificacao = ultimo?.VerificadoEm,
                IncidenteAberto = incidenteAberto != null,
                IncidenteDesde = incidenteAberto?.IniciadoEm,
                HistoricoStatus = checks.Take(10).Select(c => new
                {
                    Status = c.Status.ToString(),
                    c.ResponseTimeMs,
                    c.VerificadoEm
                })
            });
        }

        var incidentesAbertos = await _db.Incidents
            .Include(i => i.UrlTarget)
            .Where(i => i.ResolvidoEm == null)
            .OrderByDescending(i => i.IniciadoEm)
            .Select(i => new
            {
                i.Id,
                NomeUrl = i.UrlTarget.Nome,
                i.UrlTarget.Url,
                i.Motivo,
                i.IniciadoEm
            })
            .ToListAsync();

        return Ok(new
        {
            AtualizadoEm = DateTime.UtcNow,
            TotalUrls = targets.Count,
            UrlsOnline = resultado.Count(r => ((dynamic)r).Status == "Online"),
            IncidentesAbertos = incidentesAbertos.Count,
            Urls = resultado,
            Incidentes = incidentesAbertos
        });
    }
}