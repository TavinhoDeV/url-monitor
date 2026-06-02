namespace UrlMonitor.Models;

public enum CheckStatus
{
    Online,
    Offline,
    Timeout,
    Error
}

public class CheckResult
{
    public int Id { get; set; }
    public int UrlTargetId { get; set; }
    public UrlTarget UrlTarget { get; set; } = null!;
    public CheckStatus Status { get; set; }
    public int? StatusCode { get; set; }
    public long ResponseTimeMs { get; set; }
    public string? Mensagem { get; set; }
    public DateTime VerificadoEm { get; set; } = DateTime.UtcNow;
}
