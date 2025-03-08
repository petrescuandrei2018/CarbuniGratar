using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarbuniGratar.Web.Migrations
{
    /// <inheritdoc />
    public partial class AdaugaUtilizatoriInactivi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UtilizatoriInactivi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataUltimeiActivitati = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motiv = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilizatoriInactivi", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UtilizatoriInactivi");
        }
    }
}
