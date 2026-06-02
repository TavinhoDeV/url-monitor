# URL Monitor — Worker Service em C#

**Worker Service** desenvolvido em **C# + .NET 8** que monitora URLs em background, detecta quedas, rastreia incidentes e gera logs com **Serilog**. Ideal para demonstrar conhecimento em processamento assíncrono e serviços de background.

---

##  Funcionalidades

-  Worker Service rodando em background continuamente
-  Verificação de múltiplas URLs em paralelo
-  Intervalo de verificação configurável por URL
-  Medição do tempo de resposta (ms)
-  Abertura e resolução automática de incidentes
-  Logs estruturados no console e em arquivo com Serilog
-  Histórico completo de checks no SQLite

---

##  Tecnologias

| Tecnologia | Uso |
|---|---|
| .NET 8 Worker Service | Serviço de background |
| Entity Framework Core 8 | ORM + migrações |
| SQLite | Persistência de dados |
| Serilog | Logs estruturados |
| HttpClient | Verificação das URLs |
| xUnit + Moq + FluentAssertions | Testes |

---

## ▶️ Como rodar

```bash
cd UrlMonitor
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

O worker inicia imediatamente e começa a monitorar as URLs configuradas no seed.

---

## ⚙️ Adicionando URLs para monitorar

Edite o seed em `Data/AppDbContext.cs` ou insira diretamente no banco SQLite:

```sql
INSERT INTO UrlTargets (Nome, Url, IntervalSeconds, TimeoutSeconds, ExpectedStatusCode, Ativo, CriadoEm)
VALUES ('Minha API', 'https://minha-api.com/health', 30, 5, 200, 1, datetime('now'));
```

---

##  Testes

```bash
cd UrlMonitor.Tests
dotnet test
```

---

##  Estrutura

```
url-monitor/
├── UrlMonitor/
│   ├── Models/
│   │   ├── UrlTarget.cs       — URL a monitorar
│   │   ├── CheckResult.cs     — Resultado de cada verificação
│   │   └── Incident.cs        — Incidente de queda
│   ├── Data/
│   │   └── AppDbContext.cs    — EF Core + Seed
│   ├── Services/
│   │   ├── HttpCheckerService.cs  — Faz o HTTP check
│   │   ├── IncidentService.cs     — Abre/fecha incidentes
│   │   └── ReportService.cs       — Gera resumo de uptime
│   ├── MonitorWorker.cs       — BackgroundService principal
│   ├── Program.cs
│   └── appsettings.json
├── UrlMonitor.Tests/
│   └── MonitorTests.cs        — 12 testes com Moq + FluentAssertions
└── UrlMonitor.sln
```

---

## 📄 Licença

MIT License
