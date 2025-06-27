using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRelacionamentoContaCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Contas_ContaId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_ContaId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "ContaId",
                table: "Clientes");

            migrationBuilder.AddColumn<Guid>(
                name: "ClienteId",
                table: "Contas",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Contas_ClienteId",
                table: "Contas",
                column: "ClienteId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Contas_Clientes_ClienteId",
                table: "Contas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contas_Clientes_ClienteId",
                table: "Contas");

            migrationBuilder.DropIndex(
                name: "IX_Contas_ClienteId",
                table: "Contas");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Contas");

            migrationBuilder.AddColumn<Guid>(
                name: "ContaId",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_ContaId",
                table: "Clientes",
                column: "ContaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Contas_ContaId",
                table: "Clientes",
                column: "ContaId",
                principalTable: "Contas",
                principalColumn: "Id");
        }
    }
}
