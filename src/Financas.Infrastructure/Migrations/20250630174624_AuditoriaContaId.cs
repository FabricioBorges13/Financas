using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AuditoriaContaId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ContaDestinoId",
                table: "Auditorias",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContaOrigemId",
                table: "Auditorias",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContaDestinoId",
                table: "Auditorias");

            migrationBuilder.DropColumn(
                name: "ContaOrigemId",
                table: "Auditorias");
        }
    }
}
