using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WpfSportsCalendar.Migrations
{
    /// <inheritdoc />
    public partial class RemovedClubs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Clubs_ClubId",
                table: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Clubs");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_ClubId",
                table: "Subscriptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clubs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ClubId",
                table: "Subscriptions",
                column: "ClubId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Clubs_ClubId",
                table: "Subscriptions",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
