using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OOP4200_Tarneeb.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventType = table.Column<string>(type: "TEXT", nullable: true),
                    EventActor = table.Column<string>(type: "TEXT", nullable: true),
                    EventDetails = table.Column<string>(type: "TEXT", nullable: true),
                    EventTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlayerName = table.Column<string>(type: "TEXT", nullable: true),
                    NumGames = table.Column<int>(type: "INTEGER", nullable: false),
                    NumWins = table.Column<int>(type: "INTEGER", nullable: false),
                    NumLosses = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stats", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Stats",
                columns: new[] { "Id", "NumGames", "NumLosses", "NumWins", "PlayerName" },
                values: new object[] { new Guid("707e21b6-9a17-440d-a2f7-626c0637170f"), 0, 0, 0, "Player 1" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Stats");
        }
    }
}
