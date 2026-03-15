using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eLetter25.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerIdToLetter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                schema: "eletter25",
                table: "Letters",
                type: "uuid",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Letters_OwnerId",
                schema: "eletter25",
                table: "Letters",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Letters_OwnerId",
                schema: "eletter25",
                table: "Letters");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                schema: "eletter25",
                table: "Letters");
        }
    }
}
