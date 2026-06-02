using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UrlMonitor.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UrlTargets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    IntervalSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    ExpectedStatusCode = table.Column<int>(type: "INTEGER", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlTargets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CheckResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UrlTargetId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StatusCode = table.Column<int>(type: "INTEGER", nullable: true),
                    ResponseTimeMs = table.Column<long>(type: "INTEGER", nullable: false),
                    Mensagem = table.Column<string>(type: "TEXT", nullable: true),
                    VerificadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckResults_UrlTargets_UrlTargetId",
                        column: x => x.UrlTargetId,
                        principalTable: "UrlTargets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Incidents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UrlTargetId = table.Column<int>(type: "INTEGER", nullable: false),
                    IniciadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ResolvidoEm = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Motivo = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incidents_UrlTargets_UrlTargetId",
                        column: x => x.UrlTargetId,
                        principalTable: "UrlTargets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "UrlTargets",
                columns: new[] { "Id", "Ativo", "CriadoEm", "ExpectedStatusCode", "IntervalSeconds", "Nome", "TimeoutSeconds", "Url" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2026, 6, 2, 22, 19, 13, 361, DateTimeKind.Utc).AddTicks(8124), 200, 60, "Google", 10, "https://www.google.com" },
                    { 2, true, new DateTime(2026, 6, 2, 22, 19, 13, 361, DateTimeKind.Utc).AddTicks(8171), 200, 120, "GitHub", 10, "https://github.com" },
                    { 3, true, new DateTime(2026, 6, 2, 22, 19, 13, 361, DateTimeKind.Utc).AddTicks(8306), 200, 60, "Exemplo Offline", 5, "https://this-url-does-not-exist-xyz.com" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheckResults_UrlTargetId",
                table: "CheckResults",
                column: "UrlTargetId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_UrlTargetId",
                table: "Incidents",
                column: "UrlTargetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckResults");

            migrationBuilder.DropTable(
                name: "Incidents");

            migrationBuilder.DropTable(
                name: "UrlTargets");
        }
    }
}
