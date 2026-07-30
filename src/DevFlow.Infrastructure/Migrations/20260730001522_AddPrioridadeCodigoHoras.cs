using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPrioridadeCodigoHoras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nivel",
                table: "Tarefas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Media");

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Tarefas",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "HoraFim",
                table: "Tarefas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HoraInicio",
                table: "Tarefas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prioridade",
                table: "Tarefas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Media");

            // Backfill: tarefas já existentes recebem um código único baseado no próprio Id
            // antes do índice único ser criado (o default "" colidiria entre todas as linhas).
            migrationBuilder.Sql(
                "UPDATE \"Tarefas\" SET \"Codigo\" = LPAD(\"Id\"::text, 6, '0') WHERE \"Codigo\" = ''");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_Codigo",
                table: "Tarefas",
                column: "Codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tarefas_Codigo",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "HoraFim",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "HoraInicio",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "Prioridade",
                table: "Tarefas");

            migrationBuilder.AlterColumn<string>(
                name: "Nivel",
                table: "Tarefas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Media",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);
        }
    }
}
