using Microsoft.EntityFrameworkCore;
using UrlMonitor.Models;

namespace UrlMonitor.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UrlTarget> UrlTargets { get; set; }
    public DbSet<CheckResult> CheckResults { get; set; }
    public DbSet<Incident> Incidents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CheckResult>(e =>
        {
            e.HasOne(c => c.UrlTarget)
             .WithMany()
             .HasForeignKey(c => c.UrlTargetId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Incident>(e =>
        {
            e.HasOne(i => i.UrlTarget)
             .WithMany()
             .HasForeignKey(i => i.UrlTargetId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UrlTarget>().HasData(
            // ── Tech ──────────────────────────────────────────────
            new UrlTarget { Id = 1, Nome = "GitHub", Url = "https://github.com", IntervalSeconds = 60, TimeoutSeconds = 10, ExpectedStatusCode = 200 },
            new UrlTarget { Id = 2, Nome = "Cloudflare", Url = "https://www.cloudflare.com", IntervalSeconds = 60, TimeoutSeconds = 10, ExpectedStatusCode = 200 },
            new UrlTarget { Id = 3, Nome = "Vercel", Url = "https://vercel.com", IntervalSeconds = 60, TimeoutSeconds = 10, ExpectedStatusCode = 200 },
            new UrlTarget { Id = 4, Nome = "NPM Registry", Url = "https://registry.npmjs.org", IntervalSeconds = 120, TimeoutSeconds = 10, ExpectedStatusCode = 200 },
            new UrlTarget { Id = 5, Nome = "Google", Url = "https://www.google.com", IntervalSeconds = 60, TimeoutSeconds = 10, ExpectedStatusCode = 200 },
            new UrlTarget { Id = 6, Nome = "AWS", Url = "https://aws.amazon.com", IntervalSeconds = 120, TimeoutSeconds = 10, ExpectedStatusCode = 200 },
            // ── Brasileiro ────────────────────────────────────────
            new UrlTarget { Id = 7, Nome = "Nubank", Url = "https://nubank.com.br", IntervalSeconds = 60, TimeoutSeconds = 10, ExpectedStatusCode = 200 },
            new UrlTarget { Id = 8, Nome = "iFood", Url = "https://www.ifood.com.br", IntervalSeconds = 60, TimeoutSeconds = 10, ExpectedStatusCode = 200 },
            new UrlTarget { Id = 9, Nome = "G1 Globo", Url = "https://g1.globo.com", IntervalSeconds = 120, TimeoutSeconds = 10, ExpectedStatusCode = 200 },
            new UrlTarget { Id = 10, Nome = "Mercado Livre", Url = "https://www.mercadolivre.com.br", IntervalSeconds = 120, TimeoutSeconds = 10, ExpectedStatusCode = 200 },
            new UrlTarget { Id = 11, Nome = "Banco do Brasil", Url = "https://www.bb.com.br", IntervalSeconds = 120, TimeoutSeconds = 10, ExpectedStatusCode = 200 },
            new UrlTarget { Id = 12, Nome = "Receita Federal", Url = "https://www.gov.br/receitafederal", IntervalSeconds = 180, TimeoutSeconds = 15, ExpectedStatusCode = 200 }
        );
    }
}