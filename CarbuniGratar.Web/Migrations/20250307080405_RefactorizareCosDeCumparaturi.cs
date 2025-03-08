using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarbuniGratar.Web.Migrations
{
    /// <inheritdoc />
    public partial class RefactorizareCosDeCumparaturi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComenziProduse");

            migrationBuilder.DropTable(
                name: "Comenzi");

            migrationBuilder.AddColumn<int>(
                name: "CosDeCumparaturiId",
                table: "Produse",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CosuriDeCumparaturi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    DataCreare = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CosuriDeCumparaturi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CosuriDeCumparaturi_Clienti_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clienti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Produse_CosDeCumparaturiId",
                table: "Produse",
                column: "CosDeCumparaturiId");

            migrationBuilder.CreateIndex(
                name: "IX_CosuriDeCumparaturi_ClientId",
                table: "CosuriDeCumparaturi",
                column: "ClientId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Produse_CosuriDeCumparaturi_CosDeCumparaturiId",
                table: "Produse",
                column: "CosDeCumparaturiId",
                principalTable: "CosuriDeCumparaturi",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produse_CosuriDeCumparaturi_CosDeCumparaturiId",
                table: "Produse");

            migrationBuilder.DropTable(
                name: "CosuriDeCumparaturi");

            migrationBuilder.DropIndex(
                name: "IX_Produse_CosDeCumparaturiId",
                table: "Produse");

            migrationBuilder.DropColumn(
                name: "CosDeCumparaturiId",
                table: "Produse");

            migrationBuilder.CreateTable(
                name: "Comenzi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    DataPlasare = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comenzi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comenzi_Clienti_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clienti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComenziProduse",
                columns: table => new
                {
                    ComandaId = table.Column<int>(type: "int", nullable: false),
                    ProdusId = table.Column<int>(type: "int", nullable: false),
                    Cantitate = table.Column<int>(type: "int", nullable: false),
                    PretUnitate = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComenziProduse", x => new { x.ComandaId, x.ProdusId });
                    table.ForeignKey(
                        name: "FK_ComenziProduse_Comenzi_ComandaId",
                        column: x => x.ComandaId,
                        principalTable: "Comenzi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComenziProduse_Produse_ProdusId",
                        column: x => x.ProdusId,
                        principalTable: "Produse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comenzi_ClientId",
                table: "Comenzi",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ComenziProduse_ProdusId",
                table: "ComenziProduse",
                column: "ProdusId");
        }
    }
}
