using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace eLetter25.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "eletter25");

            migrationBuilder.CreateTable(
                name: "Letters",
                schema: "eletter25",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Subject = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SentDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SenderReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    RecipientReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    SenderName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SenderStreet = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SenderPostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SenderCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SenderCountry = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SenderEmail = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    SenderPhone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RecipientName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RecipientStreet = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RecipientPostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RecipientCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RecipientCountry = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RecipientEmail = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    RecipientPhone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Letters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LetterDocuments",
                schema: "eletter25",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LetterId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentFormat = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ContentHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LetterDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LetterDocuments_Letters_LetterId",
                        column: x => x.LetterId,
                        principalSchema: "eletter25",
                        principalTable: "Letters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LetterTags",
                schema: "eletter25",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LetterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tag = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LetterTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LetterTags_Letters_LetterId",
                        column: x => x.LetterId,
                        principalSchema: "eletter25",
                        principalTable: "Letters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LetterDocuments_LetterId",
                schema: "eletter25",
                table: "LetterDocuments",
                column: "LetterId");

            migrationBuilder.CreateIndex(
                name: "IX_LetterTags_LetterId",
                schema: "eletter25",
                table: "LetterTags",
                column: "LetterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LetterDocuments",
                schema: "eletter25");

            migrationBuilder.DropTable(
                name: "LetterTags",
                schema: "eletter25");

            migrationBuilder.DropTable(
                name: "Letters",
                schema: "eletter25");
        }
    }
}
