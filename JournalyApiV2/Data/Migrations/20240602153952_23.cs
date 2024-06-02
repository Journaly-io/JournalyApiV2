using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace JournalyApiV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class _23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptedDEK",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "KEKSalt",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "EncryptedDEKType",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncryptedDEKType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EncryptedDEK",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Owner = table.Column<Guid>(type: "uuid", nullable: false),
                    Salt = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncryptedDEK", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EncryptedDEK_EncryptedDEKType_Type",
                        column: x => x.Type,
                        principalTable: "EncryptedDEKType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EncryptedDEK_Type",
                table: "EncryptedDEK",
                column: "Type");
            
            // Seed EncryptedDEKType table
            migrationBuilder.InsertData(
                table: "EncryptedDEKType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Primary" }
            );
            migrationBuilder.InsertData(
                table: "EncryptedDEKType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "QR" }
            );
            migrationBuilder.InsertData(
                table: "EncryptedDEKType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Cloud" }
            );
            migrationBuilder.InsertData(
                table: "EncryptedDEKType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4, "Manual" }
            );
            migrationBuilder.InsertData(
                table: "EncryptedDEKType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 5, "Hardware" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EncryptedDEK");

            migrationBuilder.DropTable(
                name: "EncryptedDEKType");

            migrationBuilder.AddColumn<string>(
                name: "EncryptedDEK",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KEKSalt",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
