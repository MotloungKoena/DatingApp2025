using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "Visits",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AppUserId1",
                table: "Visits",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visits_AppUserId",
                table: "Visits",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_AppUserId1",
                table: "Visits",
                column: "AppUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_AspNetUsers_AppUserId",
                table: "Visits",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_AspNetUsers_AppUserId1",
                table: "Visits",
                column: "AppUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_AspNetUsers_AppUserId",
                table: "Visits");

            migrationBuilder.DropForeignKey(
                name: "FK_Visits_AspNetUsers_AppUserId1",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Visits_AppUserId",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Visits_AppUserId1",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "AppUserId1",
                table: "Visits");
        }
    }
}
