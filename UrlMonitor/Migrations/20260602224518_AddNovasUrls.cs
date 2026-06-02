using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UrlMonitor.Migrations
{
    /// <inheritdoc />
    public partial class AddNovasUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UrlTargets",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CriadoEm", "Nome", "Url" },
                values: new object[] { new DateTime(2026, 6, 2, 22, 45, 17, 581, DateTimeKind.Utc).AddTicks(3724), "GitHub", "https://github.com" });

            migrationBuilder.UpdateData(
                table: "UrlTargets",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CriadoEm", "IntervalSeconds", "Nome", "Url" },
                values: new object[] { new DateTime(2026, 6, 2, 22, 45, 17, 581, DateTimeKind.Utc).AddTicks(3730), 60, "Cloudflare", "https://www.cloudflare.com" });

            migrationBuilder.UpdateData(
                table: "UrlTargets",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CriadoEm", "Nome", "TimeoutSeconds", "Url" },
                values: new object[] { new DateTime(2026, 6, 2, 22, 45, 17, 581, DateTimeKind.Utc).AddTicks(3733), "Vercel", 10, "https://vercel.com" });

            migrationBuilder.InsertData(
                table: "UrlTargets",
                columns: new[] { "Id", "Ativo", "CriadoEm", "ExpectedStatusCode", "IntervalSeconds", "Nome", "TimeoutSeconds", "Url" },
                values: new object[,]
                {
                    { 4, true, new DateTime(2026, 6, 2, 22, 45, 17, 581, DateTimeKind.Utc).AddTicks(3736), 200, 120, "NPM Registry", 10, "https://registry.npmjs.org" },
                    { 5, true, new DateTime(2026, 6, 2, 22, 45, 17, 581, DateTimeKind.Utc).AddTicks(3738), 200, 60, "Google", 10, "https://www.google.com" },
                    { 6, true, new DateTime(2026, 6, 2, 22, 45, 17, 581, DateTimeKind.Utc).AddTicks(3741), 200, 120, "AWS", 10, "https://aws.amazon.com" },
                    { 7, true, new DateTime(2026, 6, 2, 22, 45, 17, 581, DateTimeKind.Utc).AddTicks(3743), 200, 60, "Nubank", 10, "https://nubank.com.br" },
                    { 8, true, new DateTime(2026, 6, 2, 22, 45, 17, 581, DateTimeKind.Utc).AddTicks(3745), 200, 60, "iFood", 10, "https://www.ifood.com.br" },
                    { 9, true, new DateTime(2026, 6, 2, 22, 45, 17, 581, DateTimeKind.Utc).AddTicks(3747), 200, 120, "G1 Globo", 10, "https://g1.globo.com" },
                    { 10, true, new DateTime(2026, 6, 2, 22, 45, 17, 581, DateTimeKind.Utc).AddTicks(3750), 200, 120, "Mercado Livre", 10, "https://www.mercadolivre.com.br" },
                    { 11, true, new DateTime(2026, 6, 2, 22, 45, 17, 581, DateTimeKind.Utc).AddTicks(3752), 200, 120, "Banco do Brasil", 10, "https://www.bb.com.br" },
                    { 12, true, new DateTime(2026, 6, 2, 22, 45, 17, 581, DateTimeKind.Utc).AddTicks(3754), 200, 180, "Receita Federal", 15, "https://www.gov.br/receitafederal" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UrlTargets",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "UrlTargets",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "UrlTargets",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "UrlTargets",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "UrlTargets",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "UrlTargets",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "UrlTargets",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "UrlTargets",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "UrlTargets",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.UpdateData(
                table: "UrlTargets",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CriadoEm", "Nome", "Url" },
                values: new object[] { new DateTime(2026, 6, 2, 22, 19, 13, 361, DateTimeKind.Utc).AddTicks(8124), "Google", "https://www.google.com" });

            migrationBuilder.UpdateData(
                table: "UrlTargets",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CriadoEm", "IntervalSeconds", "Nome", "Url" },
                values: new object[] { new DateTime(2026, 6, 2, 22, 19, 13, 361, DateTimeKind.Utc).AddTicks(8171), 120, "GitHub", "https://github.com" });

            migrationBuilder.UpdateData(
                table: "UrlTargets",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CriadoEm", "Nome", "TimeoutSeconds", "Url" },
                values: new object[] { new DateTime(2026, 6, 2, 22, 19, 13, 361, DateTimeKind.Utc).AddTicks(8306), "Exemplo Offline", 5, "https://this-url-does-not-exist-xyz.com" });
        }
    }
}
