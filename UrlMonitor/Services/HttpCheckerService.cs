using System.Diagnostics;
using System.Net.Http;
using UrlMonitor.Models;

namespace UrlMonitor.Services;

public class HttpCheckerService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HttpCheckerService> _logger;

    public HttpCheckerService(IHttpClientFactory httpClientFactory, ILogger<HttpCheckerService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<CheckResult> CheckAsync(UrlTarget target)
    {
        var client = _httpClientFactory.CreateClient("monitor");
        client.Timeout = TimeSpan.FromSeconds(target.TimeoutSeconds);

        var result = new CheckResult
        {
            UrlTargetId = target.Id,
            VerificadoEm = DateTime.UtcNow,
        };

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await client.GetAsync(target.Url);
            stopwatch.Stop();

            result.StatusCode = (int)response.StatusCode;
            result.ResponseTimeMs = stopwatch.ElapsedMilliseconds;

            if ((int)response.StatusCode == target.ExpectedStatusCode)
            {
                result.Status = CheckStatus.Online;
                result.Mensagem = $"OK — {stopwatch.ElapsedMilliseconds}ms";
                _logger.LogInformation("✅ {Nome} ({Url}) — {Ms}ms", target.Nome, target.Url, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                result.Status = CheckStatus.Offline;
                result.Mensagem = $"Status inesperado: {(int)response.StatusCode} (esperado: {target.ExpectedStatusCode})";
                _logger.LogWarning("⚠️  {Nome} ({Url}) — Status {Code}", target.Nome, target.Url, (int)response.StatusCode);
            }
        }
        catch (TaskCanceledException)
        {
            stopwatch.Stop();
            result.Status = CheckStatus.Timeout;
            result.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            result.Mensagem = $"Timeout apos {target.TimeoutSeconds}s";
            _logger.LogWarning("⏱️  {Nome} ({Url}) — Timeout", target.Nome, target.Url);
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            result.Status = CheckStatus.Error;
            result.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            result.Mensagem = $"Erro de conexao: {ex.Message}";
            _logger.LogError("🔴 {Nome} ({Url}) — {Erro}", target.Nome, target.Url, ex.Message);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.Status = CheckStatus.Error;
            result.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            result.Mensagem = $"Erro inesperado: {ex.Message}";
            _logger.LogError(ex, "🔴 {Nome} ({Url}) — Erro inesperado", target.Nome, target.Url);
        }

        return result;
    }
}
