using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JournalyApiV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmotionCategory",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AllowMultiple = table.Column<bool>(type: "boolean", nullable: false),
                    Default = table.Column<bool>(type: "boolean", nullable: false),
                    Order = table.Column<short>(type: "smallint", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Owner = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmotionCategory", x => x.Uuid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmotionCategory");
        }
    }
}
