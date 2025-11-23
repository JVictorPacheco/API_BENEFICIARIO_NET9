using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Beneficiarios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Planos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NomePlano = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CodRegistroAns = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StatusPlano = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    DataCadastro = table.Column<DateTime>(type: "timestamp", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DataExclusao = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Beneficiarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CPF = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PlanoId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DataExclusao = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beneficiarios_Planos",
                        column: x => x.PlanoId,
                        principalTable: "Planos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiarios_CPF",
                table: "Beneficiarios",
                column: "CPF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiarios_Excluido",
                table: "Beneficiarios",
                column: "Excluido");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiarios_PlanoId",
                table: "Beneficiarios",
                column: "PlanoId");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiarios_Status",
                table: "Beneficiarios",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiarios_Status_PlanoId",
                table: "Beneficiarios",
                columns: new[] { "Status", "PlanoId" });

            migrationBuilder.CreateIndex(
                name: "IX_Planos_CodRegistroANS",
                table: "Planos",
                column: "CodRegistroAns",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Planos_Excluido",
                table: "Planos",
                column: "Excluido");

            migrationBuilder.CreateIndex(
                name: "IX_Planos_Nome",
                table: "Planos",
                column: "NomePlano",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Planos_StatusPlano",
                table: "Planos",
                column: "StatusPlano");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Beneficiarios");

            migrationBuilder.DropTable(
                name: "Planos");
        }
    }
}
