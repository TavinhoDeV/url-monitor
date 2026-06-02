using System.Net;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using UrlMonitor.Data;
using UrlMonitor.Models;
using UrlMonitor.Services;

namespace UrlMonitor.Tests;

public class HttpCheckerServiceTests
{
    private static HttpCheckerService CriarService(HttpStatusCode statusCode, bool throwTimeout = false, bool throwConnectionError = false)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var setup = handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());

        if (throwTimeout)
            setup.ThrowsAsync(new TaskCanceledException("Timeout"));
        else if (throwConnectionError)
            setup.ThrowsAsync(new HttpRequestException("Connection refused"));
        else
            setup.ReturnsAsync(new HttpResponseMessage(statusCode));

        var client = new HttpClient(handlerMock.Object);
        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        return new HttpCheckerService(factoryMock.Object, NullLogger<HttpCheckerService>.Instance);
    }

    private static UrlTarget CriarTarget(int expectedStatus = 200) => new()
    {
        Id = 1,
        Nome = "Teste",
        Url = "https://example.com",
        TimeoutSeconds = 5,
        ExpectedStatusCode = expectedStatus
    };

    // ── Testes de status ──────────────────────────────────────────

    [Fact]
    public async Task Check_DeveRetornarOnline_QuandoStatusEsperado()
    {
        var service = CriarService(HttpStatusCode.OK);
        var resultado = await service.CheckAsync(CriarTarget(200));

        resultado.Status.Should().Be(CheckStatus.Online);
        resultado.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Check_DeveRetornarOffline_QuandoStatusDiferente()
    {
        var service = CriarService(HttpStatusCode.NotFound);
        var resultado = await service.CheckAsync(CriarTarget(200));

        resultado.Status.Should().Be(CheckStatus.Offline);
        resultado.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Check_DeveRetornarOffline_QuandoServerError()
    {
        var service = CriarService(HttpStatusCode.InternalServerError);
        var resultado = await service.CheckAsync(CriarTarget(200));

        resultado.Status.Should().Be(CheckStatus.Offline);
        resultado.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task Check_DeveRetornarTimeout_QuandoTaskCancelada()
    {
        var service = CriarService(HttpStatusCode.OK, throwTimeout: true);
        var resultado = await service.CheckAsync(CriarTarget());

        resultado.Status.Should().Be(CheckStatus.Timeout);
        resultado.Mensagem.Should().Contain("Timeout");
    }

    [Fact]
    public async Task Check_DeveRetornarError_QuandoConexaoRecusada()
    {
        var service = CriarService(HttpStatusCode.OK, throwConnectionError: true);
        var resultado = await service.CheckAsync(CriarTarget());

        resultado.Status.Should().Be(CheckStatus.Error);
        resultado.Mensagem.Should().Contain("conexao");
    }

    [Fact]
    public async Task Check_DeveRegistrarTempoDeResposta()
    {
        var service = CriarService(HttpStatusCode.OK);
        var resultado = await service.CheckAsync(CriarTarget());

        resultado.ResponseTimeMs.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Check_DeveVincularUrlTargetId()
    {
        var service = CriarService(HttpStatusCode.OK);
        var target = CriarTarget();
        var resultado = await service.CheckAsync(target);

        resultado.UrlTargetId.Should().Be(target.Id);
    }
}

public class IncidentServiceTests
{
    private AppDbContext CriarDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static CheckResult CriarResultado(int targetId, CheckStatus status) => new()
    {
        UrlTargetId = targetId,
        Status = status,
        ResponseTimeMs = 100,
        VerificadoEm = DateTime.UtcNow
    };

    [Fact]
    public async Task Processar_DeveAbrirIncidente_QuandoUrlFicarOffline()
    {
        var db = CriarDb();
        var service = new IncidentService(db, NullLogger<IncidentService>.Instance);

        await service.ProcessarAsync(CriarResultado(1, CheckStatus.Offline));

        var incidentes = await db.Incidents.ToListAsync();
        incidentes.Should().HaveCount(1);
        incidentes[0].ResolvidoEm.Should().BeNull();
    }

    [Fact]
    public async Task Processar_NaoDeveAbrirDuplicado_QuandoJaExisteIncidenteAberto()
    {
        var db = CriarDb();
        var service = new IncidentService(db, NullLogger<IncidentService>.Instance);

        await service.ProcessarAsync(CriarResultado(1, CheckStatus.Offline));
        await service.ProcessarAsync(CriarResultado(1, CheckStatus.Offline));

        var incidentes = await db.Incidents.ToListAsync();
        incidentes.Should().HaveCount(1);
    }

    [Fact]
    public async Task Processar_DeveResolverIncidente_QuandoUrlVoltar()
    {
        var db = CriarDb();
        var service = new IncidentService(db, NullLogger<IncidentService>.Instance);

        await service.ProcessarAsync(CriarResultado(1, CheckStatus.Offline));
        await service.ProcessarAsync(CriarResultado(1, CheckStatus.Online));

        var incidente = await db.Incidents.FirstAsync();
        incidente.ResolvidoEm.Should().NotBeNull();
    }

    [Fact]
    public async Task Processar_NaoDeveAbrirIncidente_QuandoUrlEstaOnline()
    {
        var db = CriarDb();
        var service = new IncidentService(db, NullLogger<IncidentService>.Instance);

        await service.ProcessarAsync(CriarResultado(1, CheckStatus.Online));

        var incidentes = await db.Incidents.ToListAsync();
        incidentes.Should().BeEmpty();
    }

    [Fact]
    public async Task ListarAbertos_DeveRetornarSomentePendentes()
    {
        var db = CriarDb();
        db.UrlTargets.Add(new UrlTarget { Id = 1, Nome = "T1", Url = "https://t1.com" });
        db.UrlTargets.Add(new UrlTarget { Id = 2, Nome = "T2", Url = "https://t2.com" });
        db.Incidents.Add(new Incident { UrlTargetId = 1, Motivo = "Offline" });
        db.Incidents.Add(new Incident { UrlTargetId = 2, Motivo = "Timeout", ResolvidoEm = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var service = new IncidentService(db, NullLogger<IncidentService>.Instance);
        var abertos = await service.ListarAbertosAsync();

        abertos.Should().HaveCount(1);
        abertos[0].UrlTargetId.Should().Be(1);
    }
}
