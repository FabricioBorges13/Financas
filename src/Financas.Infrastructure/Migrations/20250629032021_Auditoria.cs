using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Auditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChaveIdempotencia",
                table: "Transacoes");

            migrationBuilder.AddColumn<int>(
                name: "Parcelas",
                table: "Transacoes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Auditorias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoTransacao = table.Column<int>(type: "int", nullable: false),
                    EntidadeAfetada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dados = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusTransacao = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditorias", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auditorias");

            migrationBuilder.DropColumn(
                name: "Parcelas",
                table: "Transacoes");

            migrationBuilder.AddColumn<Guid>(
                name: "ChaveIdempotencia",
                table: "Transacoes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
