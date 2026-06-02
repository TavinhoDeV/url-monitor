namespace UrlMonitor.Models;

public class Incident
{
    public int Id { get; set; }
    public int UrlTargetId { get; set; }
    public UrlTarget UrlTarget { get; set; } = null!;
    public DateTime IniciadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvidoEm { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public bool Resolvido => ResolvidoEm.HasValue;

    public TimeSpan? Duracao => ResolvidoEm.HasValue
        ? ResolvidoEm.Value - IniciadoEm
        : DateTime.UtcNow - IniciadoEm;
}
