using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HubGruposEstudo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarDominioCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "usuarios",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Nivel",
                table: "usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Xp",
                table: "usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "conquistas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Icone = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Criterio = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conquistas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "habilidades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Categoria = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_habilidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "conquistas_usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConquistaId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataObtida = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conquistas_usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_conquistas_usuarios_conquistas_ConquistaId",
                        column: x => x.ConquistaId,
                        principalTable: "conquistas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_conquistas_usuarios_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trilhas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    NivelDificuldade = table.Column<string>(type: "text", nullable: false),
                    CriadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    HabilidadeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trilhas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trilhas_habilidades_HabilidadeId",
                        column: x => x.HabilidadeId,
                        principalTable: "habilidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trilhas_usuarios_CriadorId",
                        column: x => x.CriadorId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usuarios_habilidades",
                columns: table => new
                {
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    HabilidadeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios_habilidades", x => new { x.UsuarioId, x.HabilidadeId });
                    table.ForeignKey(
                        name: "FK_usuarios_habilidades_habilidades_HabilidadeId",
                        column: x => x.HabilidadeId,
                        principalTable: "habilidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_usuarios_habilidades_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "etapas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrilhaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    DuracaoEstimadaMin = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_etapas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_etapas_trilhas_TrilhaId",
                        column: x => x.TrilhaId,
                        principalTable: "trilhas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inscricoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrilhaId = table.Column<Guid>(type: "uuid", nullable: false),
                    EtapaAtual = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Concluida = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataConclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inscricoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_inscricoes_trilhas_TrilhaId",
                        column: x => x.TrilhaId,
                        principalTable: "trilhas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inscricoes_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trocas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioAId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioBId = table.Column<Guid>(type: "uuid", nullable: false),
                    HabilidadeAId = table.Column<Guid>(type: "uuid", nullable: false),
                    HabilidadeBId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrilhaAId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrilhaBId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trocas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trocas_habilidades_HabilidadeAId",
                        column: x => x.HabilidadeAId,
                        principalTable: "habilidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trocas_habilidades_HabilidadeBId",
                        column: x => x.HabilidadeBId,
                        principalTable: "habilidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trocas_trilhas_TrilhaAId",
                        column: x => x.TrilhaAId,
                        principalTable: "trilhas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trocas_trilhas_TrilhaBId",
                        column: x => x.TrilhaBId,
                        principalTable: "trilhas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trocas_usuarios_UsuarioAId",
                        column: x => x.UsuarioAId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trocas_usuarios_UsuarioBId",
                        column: x => x.UsuarioBId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_conquistas_usuarios_ConquistaId",
                table: "conquistas_usuarios",
                column: "ConquistaId");

            migrationBuilder.CreateIndex(
                name: "IX_conquistas_usuarios_UsuarioId",
                table: "conquistas_usuarios",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_etapas_TrilhaId",
                table: "etapas",
                column: "TrilhaId");

            migrationBuilder.CreateIndex(
                name: "IX_habilidades_Nome",
                table: "habilidades",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_inscricoes_TrilhaId",
                table: "inscricoes",
                column: "TrilhaId");

            migrationBuilder.CreateIndex(
                name: "IX_inscricoes_UsuarioId_TrilhaId",
                table: "inscricoes",
                columns: new[] { "UsuarioId", "TrilhaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trilhas_CriadorId",
                table: "trilhas",
                column: "CriadorId");

            migrationBuilder.CreateIndex(
                name: "IX_trilhas_HabilidadeId",
                table: "trilhas",
                column: "HabilidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_trocas_HabilidadeAId",
                table: "trocas",
                column: "HabilidadeAId");

            migrationBuilder.CreateIndex(
                name: "IX_trocas_HabilidadeBId",
                table: "trocas",
                column: "HabilidadeBId");

            migrationBuilder.CreateIndex(
                name: "IX_trocas_TrilhaAId",
                table: "trocas",
                column: "TrilhaAId");

            migrationBuilder.CreateIndex(
                name: "IX_trocas_TrilhaBId",
                table: "trocas",
                column: "TrilhaBId");

            migrationBuilder.CreateIndex(
                name: "IX_trocas_UsuarioAId",
                table: "trocas",
                column: "UsuarioAId");

            migrationBuilder.CreateIndex(
                name: "IX_trocas_UsuarioBId",
                table: "trocas",
                column: "UsuarioBId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_habilidades_HabilidadeId",
                table: "usuarios_habilidades",
                column: "HabilidadeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "conquistas_usuarios");

            migrationBuilder.DropTable(
                name: "etapas");

            migrationBuilder.DropTable(
                name: "inscricoes");

            migrationBuilder.DropTable(
                name: "trocas");

            migrationBuilder.DropTable(
                name: "usuarios_habilidades");

            migrationBuilder.DropTable(
                name: "conquistas");

            migrationBuilder.DropTable(
                name: "trilhas");

            migrationBuilder.DropTable(
                name: "habilidades");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "Nivel",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "Xp",
                table: "usuarios");
        }
    }
}
