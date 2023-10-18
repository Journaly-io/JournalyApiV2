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
                name: "IconType",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IconType", x => x.Id);
                });
            
            // Seed the IconType table with default values
            migrationBuilder.InsertData(
                table: "IconType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "twemoji" }
            );

            migrationBuilder.InsertData(
                table: "IconType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "fontawesome" }
            );
            
            migrationBuilder.CreateTable(
                name: "Emotion",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Owner = table.Column<Guid>(type: "uuid", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: false),
                    IconTypeId = table.Column<short>(type: "smallint", nullable: false),
                    Order = table.Column<short>(type: "smallint", nullable: false),
                    CategoryUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    EmotionCategoryUuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emotion", x => x.Uuid);
                    table.ForeignKey(
                        name: "FK_Emotion_EmotionCategory_EmotionCategoryUuid",
                        column: x => x.EmotionCategoryUuid,
                        principalTable: "EmotionCategory",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Emotion_IconType_IconTypeId",
                        column: x => x.IconTypeId,
                        principalTable: "IconType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emotion_EmotionCategoryUuid",
                table: "Emotion",
                column: "EmotionCategoryUuid");

            migrationBuilder.CreateIndex(
                name: "IX_Emotion_IconTypeId",
                table: "Emotion",
                column: "IconTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Emotion");

            migrationBuilder.DropTable(
                name: "IconType");
        }
    }
}
