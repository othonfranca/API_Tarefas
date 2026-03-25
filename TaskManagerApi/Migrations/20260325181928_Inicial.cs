using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManagerApi.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Colaboradores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colaboradores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tarefas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ColaboradorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarefas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tarefas_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Colaboradores",
                columns: new[] { "Id", "Email", "Nome", "Senha" },
                values: new object[,]
                {
                    { 1, "pam@example.com", "Pam", "senha123" },
                    { 2, "jim@example.com", "Jim", "senha456" },
                    { 3, "kevin@example.com", "Kevin", "senha789" },
                    { 4, "kelly@example.com", "Kelly", "senha012" },
                    { 5, "dwight@example.com", "Dwight", "senha345" },
                    { 6, "michael@example.com", "Michael", "senha678" },
                    { 7, "angela@example.com", "Angela", "senha901" },
                    { 8, "stanley@example.com", "Stanley", "senha234" },
                    { 9, "oscar@example.com", "Oscar", "senha567" },
                    { 10, "phyllis@example.com", "Phyllis", "senha890" }
                });

            migrationBuilder.InsertData(
                table: "Tarefas",
                columns: new[] { "Id", "ColaboradorId", "DataCriacao", "Descricao", "Status", "Titulo" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 3, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Descrição da tarefa 1", 0, "Alimentar o cachorro" },
                    { 2, 2, new DateTime(2026, 3, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Descrição da tarefa 2", 1, "Fazer compras" },
                    { 3, 3, new DateTime(2026, 3, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Descrição da tarefa 3", 2, "Ir à padaria" },
                    { 4, 4, new DateTime(2026, 3, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Descrição da tarefa 4", 0, "Ir à feira" },
                    { 5, 5, new DateTime(2026, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Descrição da tarefa 5", 1, "Comprar itens de festa" },
                    { 6, 6, new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Descrição da tarefa 6", 2, "Contratar palhaço" },
                    { 7, 7, new DateTime(2026, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Descrição da tarefa 7", 0, "Limpar o escritório" },
                    { 8, 8, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Descrição da tarefa 8", 1, "Preparar a festa" },
                    { 9, 1, new DateTime(2026, 4, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Descrição da tarefa 9", 2, "Preparar comida" },
                    { 10, 2, new DateTime(2026, 4, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Descrição da tarefa 10", 0, "Preparar decoração" },
                    { 11, 3, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Descrição da tarefa 11", 1, "Comer" },
                    { 12, 4, new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Descrição da tarefa 12", 2, "Fofocar" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_ColaboradorId",
                table: "Tarefas",
                column: "ColaboradorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tarefas");

            migrationBuilder.DropTable(
                name: "Colaboradores");
        }
    }
}
