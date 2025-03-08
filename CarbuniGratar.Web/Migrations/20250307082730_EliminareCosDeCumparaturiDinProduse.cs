using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarbuniGratar.Web.Migrations
{
    /// <inheritdoc />
    public partial class EliminareCosDeCumparaturiDinProduse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produse_CosuriDeCumparaturi_CosDeCumparaturiId",
                table: "Produse");

            migrationBuilder.DropIndex(
                name: "IX_Produse_CosDeCumparaturiId",
                table: "Produse");

            migrationBuilder.DropColumn(
                name: "CosDeCumparaturiId",
                table: "Produse");

            migrationBuilder.AddColumn<string>(
                name: "ProduseJson",
                table: "CosuriDeCumparaturi",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProduseJson",
                table: "CosuriDeCumparaturi");

            migrationBuilder.AddColumn<int>(
                name: "CosDeCumparaturiId",
                table: "Produse",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produse_CosDeCumparaturiId",
                table: "Produse",
                column: "CosDeCumparaturiId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produse_CosuriDeCumparaturi_CosDeCumparaturiId",
                table: "Produse",
                column: "CosDeCumparaturiId",
                principalTable: "CosuriDeCumparaturi",
                principalColumn: "Id");
        }
    }
}
