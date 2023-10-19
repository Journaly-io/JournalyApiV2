using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace JournalyApiV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class _1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JournalEntry",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Owner = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntry", x => x.Uuid);
                });

            migrationBuilder.CreateTable(
                name: "ActivityEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActivityUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    JournalEntryUuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityEntry_Activity_ActivityUuid",
                        column: x => x.ActivityUuid,
                        principalTable: "Activity",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityEntry_JournalEntry_JournalEntryUuid",
                        column: x => x.JournalEntryUuid,
                        principalTable: "JournalEntry",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmotionEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmotionUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    JournalEntryUuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmotionEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmotionEntry_Emotion_EmotionUuid",
                        column: x => x.EmotionUuid,
                        principalTable: "Emotion",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmotionEntry_JournalEntry_JournalEntryUuid",
                        column: x => x.JournalEntryUuid,
                        principalTable: "JournalEntry",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntryCategoryValue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JournalEntryUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntryCategoryValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalEntryCategoryValue_EmotionCategory_CategoryUuid",
                        column: x => x.CategoryUuid,
                        principalTable: "EmotionCategory",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalEntryCategoryValue_JournalEntry_JournalEntryUuid",
                        column: x => x.JournalEntryUuid,
                        principalTable: "JournalEntry",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityEntry_ActivityUuid",
                table: "ActivityEntry",
                column: "ActivityUuid");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityEntry_JournalEntryUuid",
                table: "ActivityEntry",
                column: "JournalEntryUuid");

            migrationBuilder.CreateIndex(
                name: "IX_EmotionEntry_EmotionUuid",
                table: "EmotionEntry",
                column: "EmotionUuid");

            migrationBuilder.CreateIndex(
                name: "IX_EmotionEntry_JournalEntryUuid",
                table: "EmotionEntry",
                column: "JournalEntryUuid");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryCategoryValue_CategoryUuid",
                table: "JournalEntryCategoryValue",
                column: "CategoryUuid");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryCategoryValue_JournalEntryUuid",
                table: "JournalEntryCategoryValue",
                column: "JournalEntryUuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityEntry");

            migrationBuilder.DropTable(
                name: "EmotionEntry");

            migrationBuilder.DropTable(
                name: "JournalEntryCategoryValue");

            migrationBuilder.DropTable(
                name: "JournalEntry");
        }
    }
}
